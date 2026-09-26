using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Items.ItemHolder;
using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace UI.FortuneWheel
{
    public class FortuneWheelWindow : UI.Window
    {
        private const int SlotsCount = 8;

        [Header("Wheel")]
        [SerializeField] private RectTransform _wheel;
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private List<WheelSlot> _slots = new();

        [Header("Level Up Effect")]
        [SerializeField] private RectTransform _levelUpTitle;
        [SerializeField, Min(0.01f)] private float _levelUpTitleIntroDuration = 0.28f;
        [SerializeField, Min(0.01f)] private float _levelUpTitlePulseDuration = 0.65f;
        [SerializeField, Range(1f, 1.25f)] private float _levelUpTitlePulseScale = 1.08f;

        [Header("Reward Pool Weights")]
        [SerializeField, Range(0, 8)] private int _itemSlots = 4;
        [SerializeField, Range(0, 8)] private int _goldSlots = 2;
        [SerializeField] private List<int> _goldAmounts = new() { 25, 50, 75, 100 };
        [SerializeField] private List<WheelBuffData> _buffLibrary = new();

        [Header("Spin")]
        [SerializeField] private float _openDelay = 0.6f;
        [SerializeField] private int _fullSpins = 5;
        [SerializeField] private float _spinDuration = 1.2f;
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
        private Coroutine _routine;
        private bool _demoPlayed;
        private Tween _spinTween;
        private AudioSource _spinAudioSource;
        private bool _isSpinning;
        private bool _isDemo;
        private Tween _stopButtonPulseTween;
        private Vector3 _stopButtonInitialScale;
        private Tween _levelUpTitleIntroTween;
        private Tween _levelUpTitlePulseTween;
        private Vector3 _levelUpTitleInitialScale;
        private bool _levelUpTitleScaleCached;

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

        private void OnEnable()
        {
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
            CancelSpin();
            PlayOpenSound();
            PlayLevelUpTitleEffect();

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine());
        }

        public void PlayDemo(CharacterInventory inventory)
        {
            Initialize(inventory);

            if (_demoPlayed)
                return;

            _demoPlayed = true;
            CancelSpin();
            _isDemo = true;
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

        private IEnumerator PlayRoutine()
        {
            PrepareWheel();

            yield return new WaitForSecondsRealtime(_openDelay);

            yield return SpinRandom(true);

            int winningIndex = DetectWinningSlotIndex();

            if (winningIndex >= 0 && winningIndex < _slots.Count)
                _slots[winningIndex].PlayPulse();

            yield return new WaitForSecondsRealtime(_holdDelayBeforeClose);

            if (winningIndex >= 0 && winningIndex < _rewards.Count)
                ApplyReward(_rewards[winningIndex]);

            Finished?.Invoke();
            _routine = null;
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

        private void StopSpin()
        {
            if (!_isSpinning || _spinTween == null)
                return;

            _spinTween.Kill(false);
            _spinTween = null;
            _isSpinning = false;
            StopButtonPulse();
            StopSpinSound();
            PlayStopSound();

            if (_isDemo && _tutorialCloseButton != null)
                _tutorialCloseButton.gameObject.SetActive(true);
        }

        private AudioSource GetAudioSource()
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();

            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            return _audioSource;
        }

        private void PlayOpenSound()
        {
            if (_openClip != null)
                GetAudioSource().PlayOneShot(_openClip);
        }

        private void StartSpinSound()
        {
            StopSpinSound();
            if (_spinClip == null)
                return;

            AudioSource source = GetSpinAudioSource();
            source.clip = _spinClip;
            source.loop = true;
            source.Play();
        }

        private AudioSource GetSpinAudioSource()
        {
            AudioSource mainSource = GetAudioSource();

            if (_spinAudioSource == null)
            {
                _spinAudioSource = gameObject.AddComponent<AudioSource>();
                _spinAudioSource.outputAudioMixerGroup = mainSource.outputAudioMixerGroup;
            }

            _spinAudioSource.playOnAwake = false;
            _spinAudioSource.spatialBlend = 0f;
            return _spinAudioSource;
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
                GetAudioSource().PlayOneShot(_stopClip);
        }

        private void CancelSpin(bool playStopSound = false)
        {
            bool wasSpinning = _isSpinning;
            _spinTween?.Kill();
            _spinTween = null;
            _isSpinning = false;
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

            int itemCount = Mathf.Clamp(_itemSlots, 0, SlotsCount);
            int goldCount = Mathf.Clamp(_goldSlots, 0, SlotsCount - itemCount);
            int buffCount = SlotsCount - itemCount - goldCount;

            _itemsHolder.GetVisualDatas(itemCount, out List<ItemVisualData> itemDatas);

            for (int i = 0; i < itemCount; i++)
            {
                ItemVisualData data = i < itemDatas.Count ? itemDatas[i] : _itemsHolder.GetVisualData();
                _rewards.Add(WheelReward.CreateItem(data));
            }

            for (int i = 0; i < goldCount; i++)
                _rewards.Add(WheelReward.CreateGold(PickGold()));

            for (int i = 0; i < buffCount; i++)
                _rewards.Add(WheelReward.CreateBuff(PickBuff()));

            Shuffle(_rewards);
        }

        private int PickGold()
        {
            if (_goldAmounts == null || _goldAmounts.Count == 0)
                return 50;

            return _goldAmounts[UnityEngine.Random.Range(0, _goldAmounts.Count)];
        }

        private WheelBuffData PickBuff()
        {
            if (_buffLibrary == null || _buffLibrary.Count == 0)
                return null;

            return _buffLibrary[UnityEngine.Random.Range(0, _buffLibrary.Count)];
        }

        private void ApplyReward(WheelReward reward)
        {
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
    }
}
