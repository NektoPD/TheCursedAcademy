using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Debuffs;
using DG.Tweening;
using Items.ItemHolder;
using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace UI.FortuneWheel
{
    public class FortuneWheelWindow : UI.Window, IPointerClickHandler
    {
        private const int SlotsCount = 8;

        [Header("Wheel")] [SerializeField] private RectTransform _wheel;
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private List<WheelSlot> _slots = new();

        [Header("Pause Button")] [SerializeField]
        private Button _pauseButton;

        [SerializeField] private WheelRewardPopup _rewardPopup;

        [Header("Level Up Effect")] [SerializeField]
        private RectTransform _levelUpTitle;

        [SerializeField, Min(0.01f)] private float _levelUpTitleIntroDuration = 0.28f;
        [SerializeField, Min(0.01f)] private float _levelUpTitlePulseDuration = 0.65f;
        [SerializeField, Range(1f, 1.25f)] private float _levelUpTitlePulseScale = 1.08f;

        [Header("Reward Distribution")]
        [Tooltip("Rows apply from their start minute. Weights are relative and do not need to add up to 100.")]
        [SerializeField] private List<RewardWeightStage> _rewardWeightTable = new()
        {
            new(0f, 75f, 15f, 10f),
            new(5f, 50f, 25f, 25f),
            new(10f, 30f, 35f, 35f),
            new(15f, 20f, 40f, 40f)
        };

        [Header("Reward Pool")]
        [SerializeField] private List<int> _goldAmounts = new() { 25, 50, 75, 100 };
        [SerializeField] private List<WheelBuffData> _buffLibrary = new();

        [Header("Spin")] [SerializeField] private float _openDelay = 0.6f;
        [SerializeField] private int _fullSpins = 5;
        [SerializeField] private float _spinDuration = 1.2f;
        [SerializeField] private float _stopDuration = 1.5f;
        [SerializeField] private Ease _spinEase = Ease.InOutCubic;
        [SerializeField] private float _holdDelayBeforeClose = 1.5f;
        [SerializeField] private AudioClip _openClip;
        [SerializeField] private AudioClip _stopClip;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _spinClip;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _tutorialCloseButton;
        [SerializeField] private float _stopButtonPulseScale = 1.1f;
        [SerializeField] private float _stopButtonPulseDuration = 0.45f;

        private readonly List<WheelReward> _rewards = new();
        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;
        private GameStartController _gameStartController;
        private Coroutine _routine;
        private bool _demoPlayed;
        private Tween _spinTween;
        private AudioSource _spinAudioSource;
        private bool _isSpinning;
        private bool _isStopping;
        private bool _isDemo;
        private Tween _stopButtonPulseTween;
        private Vector3 _stopButtonInitialScale;
        private Tween _levelUpTitleIntroTween;
        private Tween _levelUpTitlePulseTween;
        private Vector3 _levelUpTitleInitialScale;
        private bool _levelUpTitleScaleCached;
        private bool _pauseButtonWasVisible;
        private float _pendingStopAngle;

        public event Action<ItemVisualData> ItemRewarded;
        public event Action<int> GoldRewarded;
        public event Action<WheelBuffData> BuffRewarded;
        public event Action Finished;
        public event Action DemoClosed;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        public void Initialize(CharacterInventory inventory) => _inventory = inventory;

        public void Initialize(CharacterInventory inventory, GameStartController gameStartController)
        {
            Initialize(inventory);
            _gameStartController = gameStartController;
        }

        private void OnEnable()
        {
            if (_rewardPopup != null)
            {
                _rewardPopup.Closed -= RestorePauseButton;
                _rewardPopup.Closed += RestorePauseButton;
            }

            if (_pauseButton != null)
            {
                _pauseButtonWasVisible |= _pauseButton.gameObject.activeSelf;
                _pauseButton.gameObject.SetActive(false);
            }

            CacheLevelUpTitle();

            if (_stopButton != null)
            {
                _stopButton.onClick.AddListener(StopSpin);
                _stopButtonInitialScale = _stopButton.transform.localScale;
            }

            if (_tutorialCloseButton != null)
            {
                _tutorialCloseButton.onClick.AddListener(CloseDemo);
                _tutorialCloseButton.gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            FastSpinFinish();
        }

        public override void OpenWindow()
        {
            base.OpenWindow();
            Play();
        }

        public override void OpenUnscaledTime()
        {
            base.OpenUnscaledTime();
            Play();
        }

        private void Play()
        {
            Play(true);
        }

        public void PlayNextSpin()
        {
            Play(true);
        }

        private void Play(bool waitForManualStop)
        {
            CancelSpin();

            if (_stopButton != null)
                _stopButton.interactable = false;

            PlayOpenSound();
            PlayLevelUpTitleEffect();

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine(waitForManualStop));
        }

        public void PlayDemo(CharacterInventory inventory)
        {
            Initialize(inventory);

            if (_demoPlayed)
                return;

            _demoPlayed = true;
            CancelSpin();
            _isDemo = true;
            if (_stopButton != null)
                _stopButton.interactable = false;

            gameObject.SetActive(true);
            PlayOpenSound();
            PlayLevelUpTitleEffect();

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayDemoRoutine());
        }

        private void OnDisable()
        {
            CancelSpin(true);
            StopLevelUpTitleEffect();
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = null;
            if (_stopButton != null)
                _stopButton.onClick.RemoveListener(StopSpin);

            if (_tutorialCloseButton != null)
                _tutorialCloseButton.onClick.RemoveListener(CloseDemo);

            RestorePauseButton();
        }

        private void OnDestroy()
        {
            if (_rewardPopup != null)
                _rewardPopup.Closed -= RestorePauseButton;
        }

        private void RestorePauseButton()
        {
            if (_pauseButton == null || !_pauseButtonWasVisible || gameObject.activeInHierarchy)
                return;

            if (_rewardPopup != null && _rewardPopup.gameObject.activeInHierarchy)
                return;

            _pauseButtonWasVisible = false;
            _pauseButton.gameObject.SetActive(true);
        }

        private void CacheLevelUpTitle()
        {
            if (_levelUpTitle == null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.GetComponent<TMP_Text>() == null)
                        continue;

                    _levelUpTitle = child as RectTransform;
                    break;
                }
            }

            if (_levelUpTitle != null && !_levelUpTitleScaleCached)
            {
                _levelUpTitleInitialScale = _levelUpTitle.localScale;
                _levelUpTitleScaleCached = true;
            }
        }

        private void PlayLevelUpTitleEffect()
        {
            CacheLevelUpTitle();
            if (_levelUpTitle == null || !_levelUpTitleScaleCached)
                return;

            StopLevelUpTitleEffect();
            _levelUpTitle.gameObject.SetActive(true);
            _levelUpTitle.localScale = _levelUpTitleInitialScale * 0.8f;

            _levelUpTitleIntroTween = DOTween.Sequence()
                .Append(_levelUpTitle.DOScale(
                        _levelUpTitleInitialScale * 1.14f,
                        Mathf.Max(0.01f, _levelUpTitleIntroDuration))
                    .SetEase(Ease.OutBack))
                .Append(_levelUpTitle.DOScale(_levelUpTitleInitialScale, 0.14f)
                    .SetEase(Ease.InOutSine))
                .SetUpdate(true)
                .OnComplete(StartLevelUpTitlePulse);
        }

        private void StartLevelUpTitlePulse()
        {
            _levelUpTitleIntroTween = null;
            if (_levelUpTitle == null || !_levelUpTitleScaleCached)
                return;

            _levelUpTitlePulseTween = _levelUpTitle
                .DOScale(
                    _levelUpTitleInitialScale * _levelUpTitlePulseScale,
                    Mathf.Max(0.01f, _levelUpTitlePulseDuration))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void StopLevelUpTitleEffect()
        {
            _levelUpTitleIntroTween?.Kill();
            _levelUpTitleIntroTween = null;

            _levelUpTitlePulseTween?.Kill();
            _levelUpTitlePulseTween = null;

            if (_levelUpTitle != null && _levelUpTitleScaleCached)
                _levelUpTitle.localScale = _levelUpTitleInitialScale;
        }

        private void StartButtonPulse()
        {
            if (_stopButton == null)
                return;

            StopButtonPulse();
            _stopButtonPulseTween = _stopButton.transform
                .DOScale(_stopButtonInitialScale * _stopButtonPulseScale, _stopButtonPulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }

        private void StopButtonPulse()
        {
            _stopButtonPulseTween?.Kill();
            _stopButtonPulseTween = null;

            if (_stopButton != null)
                _stopButton.transform.localScale = _stopButtonInitialScale;
        }

        public void StopDemo()
        {
            CancelSpin(true);
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            if (_wheel != null)
                _wheel.DOKill();

            StopSlotPulses();
            gameObject.SetActive(false);
        }

        private void CloseDemo()
        {
            _isDemo = false;
            DemoClosed?.Invoke();
            CloseUnscaledTime();
        }

        private IEnumerator PlayRoutine(bool waitForManualStop)
        {
            PrepareWheel();

            yield return new WaitForSecondsRealtime(_openDelay);

            yield return SpinRandom(waitForManualStop);

            int winningIndex = DetectWinningSlotIndex();
            
            if (winningIndex >= 0 && winningIndex < _slots.Count)
                _slots[winningIndex].PlayPulse();

            yield return new WaitForSecondsRealtime(_holdDelayBeforeClose);

            if (winningIndex >= 0 && winningIndex < _rewards.Count)
                ApplyReward(_rewards[winningIndex]);
            
            _routine = null;
            Finished?.Invoke();
            Debug.Log("Finished");
        }

        public void FastSpinFinish()
        {
            if (!_isStopping)
                return;

            _spinTween?.Kill(false);

            if (_wheel != null)
                _wheel.localRotation = Quaternion.Euler(0f, 0f, _pendingStopAngle);

            _spinTween = null;
            _isStopping = false;
            _isSpinning = false;
        }

        private void StopSpin()
        {
            if (!_isSpinning || _isStopping || _spinTween == null)
                return;

            _isStopping = true;
            if (_stopButton != null)
                _stopButton.interactable = false;

            _spinTween.Kill(false);
            StopButtonPulse();

            float angularSpeed = 360f / Mathf.Max(0.01f, _spinDuration);
            float stoppingAngle = angularSpeed * _stopDuration / 3f;

            _pendingStopAngle = _wheel.localEulerAngles.z - stoppingAngle;

            _spinTween = _wheel
                .DORotate(new Vector3(0f, 0f, -stoppingAngle), _stopDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _isStopping = false;
                    _isSpinning = false;
                    StopSpinSound();
                    PlayStopSound();
                });
        }

        private IEnumerator PlayDemoRoutine()
        {
            yield return new WaitForSecondsRealtime(_openDelay);
            yield return SpinRandom(true);

            int winningIndex = DetectWinningSlotIndex();

            if (winningIndex >= 0 && winningIndex < _slots.Count)
                _slots[winningIndex].PlayPulse();

            if (_isDemo && _tutorialCloseButton != null)
                _tutorialCloseButton.gameObject.SetActive(true);

            _routine = null;
        }

        public void PrepareWheel()
        {
            BuildRewards();
            StopSlotPulses();

            for (int i = 0; i < _slots.Count && i < _rewards.Count; i++)
                _slots[i].Set(_rewards[i], IsNewItem(_rewards[i]));

            if (_wheel == null)
                return;

            _wheel.DOKill();
            _wheel.localScale = Vector3.one;
            _wheel.localRotation = Quaternion.identity;
        }

        private void StopSlotPulses()
        {
            for (int i = 0; i < _slots.Count; i++)
                _slots[i].StopPulse();
        }

        private IEnumerator SpinRandom(bool waitForManualStop)
        {
            if (_wheel == null)
                yield break;

            float randomOffset = UnityEngine.Random.Range(0f, 360f);
            float finalAngle = _fullSpins * 360f + randomOffset;

            _isSpinning = true;
            if (_stopButton != null)
                _stopButton.interactable = waitForManualStop;

            StartSpinSound();
            StartButtonPulse();

            if (waitForManualStop)
            {
                _spinTween = _wheel
                    .DORotate(new Vector3(0f, 0f, -360f), _spinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetRelative()
                    .SetLoops(-1, LoopType.Incremental)
                    .SetUpdate(true);

                while (_isSpinning)
                    yield return null;
            }
            else
            {
                bool done = false;
                _spinTween = _wheel
                    .DORotate(new Vector3(0f, 0f, -finalAngle), _spinDuration, RotateMode.FastBeyond360)
                    .SetEase(_spinEase)
                    .SetUpdate(true)
                    .OnComplete(() => done = true);

                while (!done && _isSpinning)
                    yield return null;

                if (_isSpinning)
                {
                    StopSpinSound();
                    StopButtonPulse();
                    PlayStopSound();
                }
            }

            _spinTween = null;
            _isSpinning = false;
        }

        private void PlayOpenSound()
        {
            _audioSource.PlayOneShot(_openClip);
        }

        private void StartSpinSound()
        {
            StopSpinSound();
            if (_spinClip == null)
                return;
            
            _audioSource.clip = _spinClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void StopSpinSound()
        {
            if (_spinAudioSource == null)
                return;

            _spinAudioSource.Stop();
            _spinAudioSource.loop = false;
        }

        private void PlayStopSound()
        {
            if (_stopClip != null)
                _audioSource.PlayOneShot(_stopClip);
        }

        private void CancelSpin(bool playStopSound = false)
        {
            bool wasSpinning = _isSpinning;
            _spinTween?.Kill();
            _spinTween = null;
            _isSpinning = false;
            _isStopping = false;
            StopSpinSound();
            StopButtonPulse();

            if (playStopSound && wasSpinning)
                PlayStopSound();
        }

        private bool IsNewItem(WheelReward reward)
        {
            if (reward == null || reward.Type != WheelRewardType.Item || reward.Item == null)
                return false;

            if (_inventory == null)
                return true;

            return _inventory.Items.All(item => item.VisualData.Variation != reward.Item.Variation);
        }

        private int DetectWinningSlotIndex()
        {
            if (_slots == null || _slots.Count == 0)
                return -1;

            Vector2 center = _wheel != null ? (Vector2)_wheel.position : Vector2.zero;
            Vector2 pointerDir = _pointer != null
                ? ((Vector2)_pointer.position - center).normalized
                : Vector2.up;

            int bestIndex = -1;
            float bestAngle = float.MaxValue;

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null)
                    continue;

                Vector2 slotDir = ((Vector2)_slots[i].transform.position - center).normalized;
                float angle = Vector2.Angle(pointerDir, slotDir);

                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private void BuildRewards()
        {
            _rewards.Clear();

            Vector3 weights = GetRewardWeights();
            List<ItemVisualData> itemDatas = _itemsHolder != null
                ? _itemsHolder.GetAvailableVisualDatas()
                : new List<ItemVisualData>();
            List<WheelBuffData> buffs = _buffLibrary != null
                ? _buffLibrary.Where(buff => buff != null).ToList()
                : new List<WheelBuffData>();

            Shuffle(itemDatas);

            for (int i = 0; i < SlotsCount; i++)
            {
                float itemWeight = itemDatas.Count > 0 ? weights.x : 0f;
                float goldWeight = weights.y;
                float buffWeight = buffs.Count > 0 ? weights.z : 0f;
                float totalWeight = itemWeight + buffWeight + goldWeight;
                float roll = totalWeight > 0f ? UnityEngine.Random.Range(0f, totalWeight) : 0f;

                if (itemWeight > 0f && roll <= itemWeight)
                {
                    int index = itemDatas.Count - 1;
                    _rewards.Add(WheelReward.CreateItem(itemDatas[index]));
                    itemDatas.RemoveAt(index);
                }
                else if (buffWeight > 0f && roll <= itemWeight + buffWeight)
                {
                    _rewards.Add(WheelReward.CreateBuff(buffs[UnityEngine.Random.Range(0, buffs.Count)]));
                }
                else
                {
                    _rewards.Add(WheelReward.CreateGold(PickGold()));
                }
            }

            Shuffle(_rewards);
        }

        private Vector3 GetRewardWeights()
        {
            if (_rewardWeightTable == null)
                return Vector3.zero;

            float elapsedMinutes = _gameStartController != null
                ? _gameStartController.ElapsedGameTime / 60f
                : 0f;
            RewardWeightStage selected = default;
            float selectedMinute = float.NegativeInfinity;

            foreach (RewardWeightStage stage in _rewardWeightTable)
            {
                if (stage.FromMinute <= elapsedMinutes && stage.FromMinute >= selectedMinute)
                {
                    selected = stage;
                    selectedMinute = stage.FromMinute;
                }
            }

            return new Vector3(
                Mathf.Max(0f, selected.ItemWeight),
                Mathf.Max(0f, selected.GoldWeight),
                Mathf.Max(0f, selected.BuffWeight));
        }

        private int PickGold()
        {
            if (_goldAmounts == null || _goldAmounts.Count == 0)
                return 50;

            return _goldAmounts[UnityEngine.Random.Range(0, _goldAmounts.Count)];
        }

        private void ApplyReward(WheelReward reward)
        {
            Debug.Log(reward);
            
            switch (reward.Type)
            {
                case WheelRewardType.Item:
                    ItemRewarded?.Invoke(reward.Item);
                    break;
                case WheelRewardType.Gold:
                    GoldRewarded?.Invoke(reward.GoldAmount);
                    break;
                case WheelRewardType.Buff:
                    BuffRewarded?.Invoke(reward.Buff);
                    break;
            }
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        [Serializable]
        private struct RewardWeightStage
        {
            [Min(0f)] public float FromMinute;
            [Min(0f)] public float ItemWeight;
            [Min(0f)] public float GoldWeight;
            [Min(0f)] public float BuffWeight;

            public RewardWeightStage(float fromMinute, float itemWeight, float goldWeight, float buffWeight)
            {
                FromMinute = fromMinute;
                ItemWeight = itemWeight;
                GoldWeight = goldWeight;
                BuffWeight = buffWeight;
            }
        }
    }
}