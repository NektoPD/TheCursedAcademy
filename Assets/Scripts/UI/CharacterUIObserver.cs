using DG.Tweening;
using CharacterLogic;
using CharacterLogic.Initializer;
using StatistiscSystem;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace UI
{
    public class CharacterUIObserver : MonoBehaviour
    {
        [SerializeField] private LevelUpWindow _levelUpWindow;
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

        private Tween _flashTween;
        private Tween _colorTween;

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

            _character = null;
        }

        private void OnNewItemAdded() => _levelUpWindow.CloseWindow();

        private void OnItemSwapped()
        {
            _levelUpWindow.CloseWindow();
            _inventoryFullWindow.CloseWindow();
        }

        private void OnItemMaxLevelReached()
        {
            _levelUpWindow.CloseUnscaledTime();
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

            if (_levelUpWindow != null)
                _levelUpWindow.Initialize(character.Inventory);

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

        private void LevelUp() => _levelUpWindow.OpenWindow();

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
            if (_vignette == null)
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
        }

        private void OnRageModeActivated()
        {
            if (_vignette == null) return;

            _vignette.color.value = _rageModeColor;
            _rageModeIntensityAdd = _rageModeIntensity;
            UpdateVignette();
        }

        private void OnRageModeDeactivated()
        {
            if (_vignette == null) return;

            _rageModeIntensityAdd = 0f;
            _vignette.color.value = _damageColor;
            UpdateVignette();
        }
    }
}