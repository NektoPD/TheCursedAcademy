using DG.Tweening;
using CharacterLogic;
using CharacterLogic.Initializer;
using Debuffs;
using StatistiscSystem;
using UI.FortuneWheel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Utils;

namespace UI
{
    public partial class CharacterUIObserver : MonoBehaviour
    {
        [SerializeField] private FortuneWheelWindow _fortuneWheelWindow;
        [SerializeField] private WheelRewardPopup _rewardPopup;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameStartController _gameStartController;
        [SerializeField] private InventoryFullWindow _inventoryFullWindow;
        [SerializeField] private StatisticsApplicator _statisticApplicator;
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private MaxLevelReachedWindow _itemMaxLevelReachedWindow;
        [SerializeField] private ExitToMenu _exit;
        [SerializeField] private Reviver _reviver;

        [Header("Post Processing")] [SerializeField]
        private PostProcessVolume _postProcessVolume;

        [Header("Vignette: Low HP")] [SerializeField, Range(0f, 1f)]
        private float _lowHealthMaxIntensity = 0.45f;

        [Header("Vignette: Flash")] [SerializeField, Range(0f, 1f)]
        private float _flashMaxAddIntensity = 0.18f;

        [SerializeField] private float _flashDuration = 0.45f;

        [SerializeField] private float _flashCooldown = 0.25f;

        [Header("Vignette Colors (HDR allowed)")] [ColorUsage(false, true)] [SerializeField]
        private Color _damageColor = Color.red;

        [ColorUsage(false, true)] [SerializeField]
        private Color _healColor = Color.green;

        [Header("Vignette: Death and Revive")]
        [SerializeField, Range(0f, 1f)] private float _deathIntensity = 0.65f;
        [SerializeField, Range(0f, 1f)] private float _reviveIntensity = 0.4f;

        [Header("Vignette: Ragemode")] [SerializeField, Range(0f, 1f)]
        private float _rageModeIntensity = 0.5f;

        [ColorUsage(false, true)] [SerializeField]
        private Color _rageModeColor = new Color(1f, 0.3f, 0f);

        private Character _character;
        private Vignette _vignette;

        private float _baseIntensity;
        private float _rageModeIntensityAdd;

        private float _flashIntensityAdd;
        private float _transitionIntensityAdd;
        private float _nextFlashTime;
        private bool _lastFlashWasHeal;

        private bool _isRageModeActive;
        private Tween _flashTween;
        private Tween _colorTween;
        private Tween _rageFadeTween;
        private Tween _transitionTween;

        private void OnEnable()
        {
            _initializer.CharacterCreated += Inizialize;
            CacheVignette();
        }

        private void LateUpdate()
        {
            if (_pauseButton == null)
                return;

            _pauseButton.interactable = !_rewardPauseHeld && !_swapPauseHeld
                && !IsWindowActive(_fortuneWheelWindow)
                && !IsWindowActive(_rewardPopup)
                && !IsWindowActive(_inventoryFullWindow)
                && !IsWindowActive(_itemMaxLevelReachedWindow);

            if (_gameStartController != null)
            {
                bool visible = _gameStartController.IsGameStarted && _pauseButton.interactable;
                if (_pauseButton.gameObject.activeSelf != visible)
                    _pauseButton.gameObject.SetActive(visible);
            }
        }

        private static bool IsWindowActive(Window window)
        {
            return window != null && window.gameObject.activeInHierarchy;
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= Inizialize;

            KillTweens();
            _flashIntensityAdd = 0f;
            _transitionIntensityAdd = 0f;
            _rageModeIntensityAdd = 0f;
            _baseIntensity = 0f;
            _isRageModeActive = false;
            _nextFlashTime = 0f;
            _lastFlashWasHeal = false;
            if (_vignette != null)
            {
                _vignette.color.value = _damageColor;
                UpdateVignette();
            }

            if (_rewardPauseHeld)
            {
                _rewardPauseHeld = false;
                GameTimeScale.SetPauseActive(false);
            }

            if (_swapPauseHeld)
            {
                _swapPauseHeld = false;
                GameTimeScale.SetPauseActive(false);
            }

            _wheelRewards.Clear();
            _pendingLevelUps = 0;
            _nextRewardIndex = 0;
            _isGrantingRewards = false;
            _waitingForItemWindow = false;

            if (_character == null)
                return;

            _character.StatisticCollected -= StatisticApplicate;
            _character.LevelUp -= LevelUp;
            _character.Damaged -= OnDamaged;
            _character.Healed -= OnHealed;
            _character.DeathStarted -= OnDeathStarted;
            _character.Revived -= OnRevived;
            _character.HealthChanged -= OnHealthChanged;
            _character.InventoryLimitReached -= InventoryLimitReached;
            _character.NewItemAdded -= OnNewItemAdded;
            _character.ItemSwapped -= OnItemSwapped;
            _character.MaxLevelReached -= OnItemMaxLevelReached;
            _character.RageModeActivated -= OnRageModeActivated;
            _character.RageModeDeactivated -= OnRageModeDeactivated;

            if (_fortuneWheelWindow != null)
            {
                _fortuneWheelWindow.ItemRewarded -= OnWheelItemRewarded;
                _fortuneWheelWindow.GoldRewarded -= OnWheelGoldRewarded;
                _fortuneWheelWindow.BuffRewarded -= OnWheelBuffRewarded;
                _fortuneWheelWindow.Finished -= OnWheelFinished;
                _fortuneWheelWindow.Closed -= OnWheelClosed;
            }

            if (_rewardPopup != null)
                _rewardPopup.Confirmed -= OnRewardPopupConfirmed;
            if (_rewardPopup != null)
                _rewardPopup.Closed -= OnRewardPopupClosed;
            if (_inventoryFullWindow != null)
                _inventoryFullWindow.Closed -= OnInventoryFullWindowClosed;
            if (_itemMaxLevelReachedWindow != null)
                _itemMaxLevelReachedWindow.Closed -= OnMaxLevelReachedWindowClosed;

            _character = null;
        }

        private void OnNewItemAdded()
        {
            if (_isGrantingRewards)
                return;

            _rewardPopup.CloseWindow();
        }

        private void OnItemSwapped()
        {
            if (!_isGrantingRewards)
                _rewardPopup.CloseWindow();

            _inventoryFullWindow.CloseUnscaledTime();
        }

        private void OnItemMaxLevelReached()
        {
            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(false);

            if (_isGrantingRewards)
                _waitingForItemWindow = true;
            else
                _rewardPopup.CloseUnscaledTime();

            _itemMaxLevelReachedWindow.OpenWindow();
        }

        private void CacheVignette()
        {
            _vignette = null;

            if (_postProcessVolume == null || _postProcessVolume.profile == null)
                return;

            if (_postProcessVolume.profile.TryGetSettings(out Vignette v))
            {
                _vignette = v;

                _vignette.enabled.value = false;
                _vignette.intensity.value = 0f;
                _vignette.color.value = _damageColor;

                _baseIntensity = 0f;
                _flashIntensityAdd = 0f;
            }
        }

        private void Inizialize(Character character)
        {
            if (_character != null)
            {
                _character.StatisticCollected -= StatisticApplicate;
                _character.LevelUp -= LevelUp;
                _character.Damaged -= OnDamaged;
                _character.Healed -= OnHealed;
                _character.DeathStarted -= OnDeathStarted;
                _character.Revived -= OnRevived;
                _character.HealthChanged -= OnHealthChanged;
                _character.InventoryLimitReached -= InventoryLimitReached;
                _character.NewItemAdded -= OnNewItemAdded;
                _character.ItemSwapped -= OnItemSwapped;
                _character.MaxLevelReached -= OnItemMaxLevelReached;
                _character.RageModeActivated -= OnRageModeActivated;
                _character.RageModeDeactivated -= OnRageModeDeactivated;
            }

            _character = character;

            if (_reviver != null)
                _reviver.Inizialize(character, _initializer);

            if (_fortuneWheelWindow != null)
            {
                _fortuneWheelWindow.Initialize(character.Inventory);
                _fortuneWheelWindow.ItemRewarded -= OnWheelItemRewarded;
                _fortuneWheelWindow.GoldRewarded -= OnWheelGoldRewarded;
                _fortuneWheelWindow.BuffRewarded -= OnWheelBuffRewarded;
                _fortuneWheelWindow.ItemRewarded += OnWheelItemRewarded;
                _fortuneWheelWindow.GoldRewarded += OnWheelGoldRewarded;
                _fortuneWheelWindow.BuffRewarded += OnWheelBuffRewarded;
                _fortuneWheelWindow.Finished -= OnWheelFinished;
                _fortuneWheelWindow.Finished += OnWheelFinished;
                _fortuneWheelWindow.Closed -= OnWheelClosed;
                _fortuneWheelWindow.Closed += OnWheelClosed;
            }

            if (_rewardPopup != null)
            {
                _rewardPopup.Confirmed -= OnRewardPopupConfirmed;
                _rewardPopup.Closed -= OnRewardPopupClosed;
                _rewardPopup.Confirmed += OnRewardPopupConfirmed;
                _rewardPopup.Closed += OnRewardPopupClosed;
            }

            if (_inventoryFullWindow != null)
            {
                _inventoryFullWindow.Initialize(character.Inventory);
                _inventoryFullWindow.Closed -= OnInventoryFullWindowClosed;
                _inventoryFullWindow.Closed += OnInventoryFullWindowClosed;
            }

            if (_itemMaxLevelReachedWindow != null)
            {
                _itemMaxLevelReachedWindow.Closed -= OnMaxLevelReachedWindowClosed;
                _itemMaxLevelReachedWindow.Closed += OnMaxLevelReachedWindowClosed;
            }

            _character.MaxLevelReached += OnItemMaxLevelReached;
            _character.StatisticCollected += StatisticApplicate;
            _character.LevelUp += LevelUp;
            _character.Damaged += OnDamaged;
            _character.Healed += OnHealed;
            _character.DeathStarted += OnDeathStarted;
            _character.Revived += OnRevived;
            _character.HealthChanged += OnHealthChanged;
            _character.InventoryLimitReached += InventoryLimitReached;
            _character.NewItemAdded += OnNewItemAdded;
            _character.ItemSwapped += OnItemSwapped;
            _character.RageModeActivated += OnRageModeActivated;
            _character.RageModeDeactivated += OnRageModeDeactivated;

            if (_vignette == null)
                CacheVignette();
        }

        private void StatisticApplicate(Statistics statistics)
        {
            if (_reviver != null)
                _reviver.HoldDeathPause();

            if (_statisticApplicator != null)
                _statisticApplicator.Applicate(statistics);

            if (_exit != null)
                _exit.SetCoins(statistics.Coins);
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_vignette == null || max <= 0f)
                return;

            float hp01 = Mathf.Clamp01(current / max);
            float severity = 1f - hp01;

            _baseIntensity = Mathf.Lerp(0f, _lowHealthMaxIntensity, severity);

            UpdateVignette();
        }

        private void OnDamaged(float current, float max)
        {
            if (_character != null && _character.IsDied)
                return;

            PlayFlash(_damageColor, false);
        }

        private void OnHealed(float current, float max)
        {
            PlayFlash(_healColor, true);
        }

        private void OnDeathStarted()
        {
            if (_vignette == null)
                return;

            KillTweens();
            _isRageModeActive = false;
            _rageModeIntensityAdd = 0f;
            _flashIntensityAdd = 0f;
            _vignette.color.value = _damageColor;
            _transitionTween = DOTween.To(
                    () => _transitionIntensityAdd,
                    value =>
                    {
                        _transitionIntensityAdd = value;
                        UpdateVignette();
                    },
                    _deathIntensity, 0.3f)
                .SetEase(Ease.OutSine).SetUpdate(true);
        }

        private void OnRevived()
        {
            if (_vignette == null)
                return;

            KillTweens();
            _isRageModeActive = false;
            _rageModeIntensityAdd = 0f;
            _flashIntensityAdd = 0f;
            _transitionIntensityAdd = _reviveIntensity;
            _vignette.color.value = _healColor;
            UpdateVignette();
            _transitionTween = DOTween.To(
                    () => _transitionIntensityAdd,
                    value =>
                    {
                        _transitionIntensityAdd = value;
                        UpdateVignette();
                    },
                    0f, 0.75f)
                .SetEase(Ease.OutSine)
                .OnComplete(ReturnToDamageColor)
                .SetUpdate(true);
        }

        private void PlayFlash(Color flashColor, bool isHeal)
        {
            if (_vignette == null || _isRageModeActive)
                return;

            if (Time.unscaledTime < _nextFlashTime && (isHeal || !_lastFlashWasHeal))
                return;

            _nextFlashTime = Time.unscaledTime + _flashCooldown;
            _lastFlashWasHeal = isHeal;

            _flashTween?.Kill();
            _colorTween?.Kill();

            _vignette.color.value = flashColor;

            _flashTween = DOTween.Sequence()
                .Append(DOTween.To(
                    () => _flashIntensityAdd,
                    x =>
                    {
                        _flashIntensityAdd = x;
                        UpdateVignette();
                    },
                    _flashMaxAddIntensity,
                    _flashDuration * 0.35f).SetEase(Ease.OutSine))
                .Append(DOTween.To(
                    () => _flashIntensityAdd,
                    x =>
                    {
                        _flashIntensityAdd = x;
                        UpdateVignette();
                    },
                    0f,
                    _flashDuration * 0.65f).SetEase(Ease.InOutSine))
                .OnComplete(ReturnToDamageColor)
                .SetUpdate(true);
        }

        private void ReturnToDamageColor()
        {
            if (_vignette == null)
                return;

            _colorTween = DOTween.To(
                    () => _vignette.color.value,
                    c => _vignette.color.value = c,
                    _damageColor,
                    0.15f)
                .SetEase(Ease.OutSine)
                .SetUpdate(true);
        }

        private void UpdateVignette()
        {
            if (_vignette == null)
                return;

            float total = Mathf.Clamp01(_baseIntensity + _flashIntensityAdd + _rageModeIntensityAdd + _transitionIntensityAdd);

            _vignette.intensity.value = total;
            _vignette.enabled.value = total > 0.001f;
        }

        private void KillTweens()
        {
            _flashTween?.Kill();
            _flashTween = null;

            _colorTween?.Kill();
            _colorTween = null;

            _rageFadeTween?.Kill();
            _rageFadeTween = null;

            _transitionTween?.Kill();
            _transitionTween = null;
        }

        private void OnRageModeActivated()
        {
            if (_vignette == null) return;

            _isRageModeActive = true;
            _rageFadeTween?.Kill();
            _flashTween?.Kill();
            _colorTween?.Kill();
            _flashIntensityAdd = 0f;
            _vignette.color.value = _rageModeColor;
            UpdateVignette();

            _rageFadeTween = DOTween.To(
                () => _rageModeIntensityAdd,
                x =>
                {
                    _rageModeIntensityAdd = x;
                    UpdateVignette();
                },
                _rageModeIntensity,
                0.4f).SetEase(Ease.OutSine).SetUpdate(true);
        }

        private void OnRageModeDeactivated()
        {
            if (_vignette == null) return;

            _isRageModeActive = false;
            _rageFadeTween?.Kill();

            _rageFadeTween = DOTween.To(
                () => _rageModeIntensityAdd,
                x =>
                {
                    _rageModeIntensityAdd = x;
                    UpdateVignette();
                },
                0f,
                0.4f)
                .SetEase(Ease.InSine)
                .OnComplete(() => _vignette.color.value = _damageColor)
                .SetUpdate(true);
        }
    }
}