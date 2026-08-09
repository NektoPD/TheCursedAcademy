using DG.Tweening;
using CharacterLogic;
using CharacterLogic.Initializer;
using StatistiscSystem;
using UI.FortuneWheel;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace UI
{
    public class CharacterUIObserver : MonoBehaviour
    {
        [SerializeField] private FortuneWheelWindow _fortuneWheelWindow;
        [SerializeField] private WheelRewardPopup _rewardPopup;
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
        private float _flashMaxAddIntensity = 0.35f;

        [SerializeField] private float _flashDuration = 0.30f;

        [Header("Vignette Colors (HDR allowed)")] [ColorUsage(false, true)] [SerializeField]
        private Color _damageColor = Color.red;

        [ColorUsage(false, true)] [SerializeField]
        private Color _healColor = Color.green;

        [Header("Vignette: Ragemode")] [SerializeField, Range(0f, 1f)]
        private float _rageModeIntensity = 0.5f;

        [ColorUsage(false, true)] [SerializeField]
        private Color _rageModeColor = new Color(1f, 0.3f, 0f);

        private Character _character;
        private Vignette _vignette;

        private float _baseIntensity;
        private float _rageModeIntensityAdd;

        private float _flashIntensityAdd;

        private bool _isRageModeActive;
        private Tween _flashTween;
        private Tween _colorTween;
        private Tween _rageFadeTween;

        private enum PendingRewardKind { None, Item, Gold, Buff }

        private PendingRewardKind _pendingKind;
        private Data.ItemVisualData _pendingItem;
        private int _pendingGold;
        private FortuneWheel.WheelBuffData _pendingBuff;

        private void OnEnable()
        {
            _initializer.CharacterCreated += Inizialize;
            CacheVignette();
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= Inizialize;

            KillTweens();

            if (_character == null)
                return;

            _character.StatisticCollected -= StatisticApplicate;
            _character.LevelUp -= LevelUp;
            _character.Damaged -= OnDamaged;
            _character.Healed -= OnHealed;
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
            }

            if (_rewardPopup != null)
                _rewardPopup.Confirmed -= OnRewardPopupConfirmed;

            _character = null;
        }

        private void OnNewItemAdded() => _rewardPopup.CloseWindow();

        private void OnItemSwapped()
        {
            _rewardPopup.CloseWindow();
            _inventoryFullWindow.CloseWindow();
        }

        private void OnItemMaxLevelReached()
        {
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
            }

            if (_rewardPopup != null)
            {
                _rewardPopup.Confirmed -= OnRewardPopupConfirmed;
                _rewardPopup.Confirmed += OnRewardPopupConfirmed;
            }

            if (_inventoryFullWindow != null)
                _inventoryFullWindow.Initialize(character.Inventory);

            _character.MaxLevelReached += OnItemMaxLevelReached;
            _character.StatisticCollected += StatisticApplicate;
            _character.LevelUp += LevelUp;
            _character.Damaged += OnDamaged;
            _character.Healed += OnHealed;
            _character.HealthChanged += OnHealthChanged;
            _character.InventoryLimitReached += InventoryLimitReached;
            _character.NewItemAdded += OnNewItemAdded;
            _character.ItemSwapped += OnItemSwapped;
            _character.RageModeActivated += OnRageModeActivated;
            _character.RageModeDeactivated += OnRageModeDeactivated;

            if (_vignette == null)
                CacheVignette();
        }

        private void LevelUp() => _fortuneWheelWindow.OpenWindow();

        private void OnWheelItemRewarded(Data.ItemVisualData item)
        {
            if (item == null)
            {
                _fortuneWheelWindow.CloseWindow();
                return;
            }

            _pendingKind = PendingRewardKind.Item;
            _pendingItem = item;
            _fortuneWheelWindow.CloseUnscaledTime();
            _rewardPopup.ShowItem(item);
        }

        private void OnWheelGoldRewarded(int amount)
        {
            _pendingKind = PendingRewardKind.Gold;
            _pendingGold = amount;
            _fortuneWheelWindow.CloseUnscaledTime();
            _rewardPopup.ShowGold(amount);
        }

        private void OnWheelBuffRewarded(FortuneWheel.WheelBuffData buff)
        {
            if (buff == null)
            {
                _fortuneWheelWindow.CloseWindow();
                return;
            }

            _pendingKind = PendingRewardKind.Buff;
            _pendingBuff = buff;
            _fortuneWheelWindow.CloseUnscaledTime();
            _rewardPopup.ShowBuff(buff);
        }

        private void OnRewardPopupConfirmed()
        {
            switch (_pendingKind)
            {
                case PendingRewardKind.Item:
                    _pendingKind = PendingRewardKind.None;
                    _character.SelectWheelItem(_pendingItem.Variation);
                    break;
                case PendingRewardKind.Gold:
                    _pendingKind = PendingRewardKind.None;
                    _character.AddWheelGold(_pendingGold);
                    _rewardPopup.CloseWindow();
                    break;
                case PendingRewardKind.Buff:
                    _pendingKind = PendingRewardKind.None;
                    _character.ApplyTemporaryBuff(_pendingBuff.Type, _pendingBuff.Multiplier, _pendingBuff.DurationSeconds);
                    _rewardPopup.CloseWindow();
                    break;
            }
        }

        private void InventoryLimitReached()
        {
            _inventoryFullWindow.OpenUnscaledTime();
        }

        private void StatisticApplicate(Statistics statistics)
        {
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
            PlayFlash(_damageColor);
        }

        private void OnHealed(float current, float max)
        {
            PlayFlash(_healColor);
        }

        private void PlayFlash(Color flashColor)
        {
            if (_vignette == null || _isRageModeActive)
                return;

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
                    _flashDuration * 0.5f).SetEase(Ease.OutSine))
                .Append(DOTween.To(
                    () => _flashIntensityAdd,
                    x =>
                    {
                        _flashIntensityAdd = x;
                        UpdateVignette();
                    },
                    0f,
                    _flashDuration * 0.5f).SetEase(Ease.InSine))
                .OnComplete(ReturnToDamageColor);
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
                .SetEase(Ease.OutSine);
        }

        private void UpdateVignette()
        {
            if (_vignette == null)
                return;

            float total = Mathf.Clamp01(_baseIntensity + _flashIntensityAdd + _rageModeIntensityAdd);

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
        }

        private void OnRageModeActivated()
        {
            if (_vignette == null) return;

            _isRageModeActive = true;
            _rageFadeTween?.Kill();
            _flashTween?.Kill();
            _vignette.color.value = _rageModeColor;

            _rageFadeTween = DOTween.To(
                () => _rageModeIntensityAdd,
                x =>
                {
                    _rageModeIntensityAdd = x;
                    UpdateVignette();
                },
                _rageModeIntensity,
                0.4f).SetEase(Ease.OutSine);
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
                .OnComplete(() => _vignette.color.value = _damageColor);
        }
    }
}