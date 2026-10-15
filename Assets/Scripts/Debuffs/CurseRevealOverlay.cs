using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public partial class CurseRevealOverlay : UI.Window, IPointerClickHandler
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _boostToggle;
        [SerializeField] private TMP_Text _boostDescription;
        [SerializeField] private TMP_Text _coinBonusText;
        [SerializeField] private CurseRevealStatsView _statsView;
        [SerializeField] private List<Image> _icons = new();
        [SerializeField] private float _charInterval = 0.04f;

        private IReadOnlyList<DebuffRoll> _debuffs;
        private IReadOnlyDictionary<PerkType, float> _baseStats;
        private float _negativeEffectIncreasePercent;
        private float _coinBonusPercent;
        private Coroutine _routine;
        private bool _isRevealing;
        private bool _choiceMade;

        public event Action<bool> Confirmed;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClosePressed);

            if (_boostToggle != null)
                _boostToggle.onValueChanged.AddListener(OnBoostChanged);

            Opened += OnOpened;
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClosePressed);

            if (_boostToggle != null)
                _boostToggle.onValueChanged.RemoveListener(OnBoostChanged);

            Opened -= OnOpened;
            StopPresentationAnimations();
            HideCurseEffectStats();

            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _isRevealing = false;
        }

        public void Show(IReadOnlyList<DebuffRoll> debuffs, float negativeEffectIncreasePercent,
            float coinBonusPercent, IReadOnlyDictionary<PerkType, float> baseStats)
        {
            StopPresentationAnimations();

            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _debuffs = debuffs;
            _baseStats = baseStats;
            _negativeEffectIncreasePercent = Mathf.Max(0f, negativeEffectIncreasePercent);
            _coinBonusPercent = Mathf.Max(0f, coinBonusPercent);
            _choiceMade = false;
            _isRevealing = false;

            if (_boostToggle != null)
                _boostToggle.SetIsOnWithoutNotify(false);

            RefreshPreview();
            if (_statsView != null)
                _statsView.PrepareAppearance();

            SetButtonsInteractable(true);
            PrepareCurseTitles(debuffs);
            PrepareIcons(debuffs);
            PrepareCurseEffectStats(debuffs, 1f);
            SetButtonsVisible(false);
            OpenUnscaledTime();
        }

        private void OnBoostChanged(bool enabled)
        {
            if (_choiceMade)
                return;

            RefreshPreview();
            float multiplier = enabled ? 1f + _negativeEffectIncreasePercent / 100f : 1f;
            PrepareCurseEffectStats(_debuffs, multiplier);
            if (!_isRevealing)
                RevealAllCurseEffectStats();
        }

        private void RefreshPreview()
        {
            bool boosted = _boostToggle != null && _boostToggle.isOn;
            float negativeEffectMultiplier = boosted ? 1f + _negativeEffectIncreasePercent / 100f : 1f;

            if (_statsView != null)
                _statsView.Show(_baseStats, _debuffs, negativeEffectMultiplier);

            if (_boostDescription != null)
                _boostDescription.text = Translator.Translate(
                    $"Проклятия сильнее на {_negativeEffectIncreasePercent:0}% за",
                    $"Stronger curses +{_negativeEffectIncreasePercent:0}% for",
                    $"Lanet gücü +%{_negativeEffectIncreasePercent:0}karşılığında");

            if (_coinBonusText != null)
                _coinBonusText.text = $"+{_coinBonusPercent:0}%";
        }

        private void OnOpened()
        {
            if (_choiceMade)
                return;

            if (_statsView != null)
                _statsView.PlayAppearance();

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(RevealRoutine());
        }

        private IEnumerator RevealRoutine()
        {
            _isRevealing = true;
            var wait = new WaitForSecondsRealtime(_charInterval);

            if (_debuffs != null)
            {
                for (int curseIndex = 0; curseIndex < _debuffs.Count; curseIndex++)
                {
                    if (_debuffs[curseIndex] == null)
                        continue;

                    if (curseIndex < _icons.Count)
                        RevealAnimatedIcon(_icons[curseIndex]);

                    CurseRevealCurseSlot slot = curseIndex < _curseSlots.Count
                        ? _curseSlots[curseIndex]
                        : null;
                    if (slot == null)
                        continue;

                    slot.RevealEffects();
                    for (int character = 0; character <= slot.TitleCharacterCount; character++)
                    {
                        slot.SetTitleVisibleCharacters(character);
                        yield return wait;
                    }
                }
            }

            _isRevealing = false;
            SetButtonsVisible(true);
            _routine = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isRevealing)
                return;

            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _isRevealing = false;
            RevealAllCurseTitles();
            RevealAllIcons();
            RevealAllCurseEffectStats();
            SetButtonsVisible(true);
        }

        private void PrepareIcons(IReadOnlyList<DebuffRoll> debuffs)
        {
            for (int i = 0; i < _icons.Count; i++)
            {
                Image icon = _icons[i];
                if (icon == null)
                    continue;

                PrepareIconAnimation(icon);
                icon.sprite = debuffs != null && i < debuffs.Count ? debuffs[i]?.Icon : null;
                icon.gameObject.SetActive(false);
            }
        }

        private void RevealAllIcons()
        {
            foreach (Image icon in _icons)
            {
                RevealAnimatedIcon(icon);
            }
        }

        private void OnClosePressed()
        {
            CloseUnscaledTime();
        }

        public override void CloseWindow()
        {
            CloseUnscaledTime();
        }

        public override void CloseUnscaledTime()
        {
            if (_choiceMade)
                return;

            _choiceMade = true;
            SetButtonsInteractable(false);
            StopPresentationAnimations();

            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _isRevealing = false;
            HideCurseEffectStats();
            Confirmed?.Invoke(_boostToggle != null && _boostToggle.isOn);
            base.CloseUnscaledTime();
        }

        private void SetButtonsVisible(bool visible)
        {
            _closeButton.gameObject.SetActive(visible);

            if (_boostToggle != null)
                _boostToggle.gameObject.SetActive(visible);
        }

        private void SetButtonsInteractable(bool interactable)
        {
            _closeButton.interactable = interactable;

            if (_boostToggle != null)
                _boostToggle.interactable = interactable;
        }
    }
}
