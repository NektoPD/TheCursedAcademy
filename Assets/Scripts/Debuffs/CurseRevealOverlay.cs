using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public partial class CurseRevealOverlay : UI.Window, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _boostToggle;
        [SerializeField] private TMP_Text _boostDescription;
        [SerializeField] private TMP_Text _coinBonusText;
        [SerializeField] private CurseRevealStatsView _statsView;
        [SerializeField] private List<Image> _icons = new();
        [SerializeField] private float _charInterval = 0.04f;

        private readonly List<int> _revealAt = new();
        private IReadOnlyList<DebuffRoll> _debuffs;
        private IReadOnlyDictionary<PerkType, float> _baseStats;
        private float _negativeEffectIncreasePercent;
        private float _coinBonusPercent;
        private Coroutine _routine;
        private bool _isRevealing;
        private bool _choiceMade;

        private string Header => Translator.Translate("Твои проклятия:", "Your Curses are:", "Lanetlerin:");

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
            _text.text = BuildText(debuffs);
            _text.ForceMeshUpdate();
            _text.maxVisibleCharacters = 0;
            PrepareIcons(debuffs);
            SetButtonsVisible(false);
            OpenUnscaledTime();
        }

        private void OnBoostChanged(bool enabled)
        {
            if (!_choiceMade)
                RefreshPreview();
        }

        private void RefreshPreview()
        {
            bool boosted = _boostToggle != null && _boostToggle.isOn;
            float negativeEffectMultiplier = boosted ? 1f + _negativeEffectIncreasePercent / 100f : 1f;

            if (_statsView != null)
                _statsView.Show(_baseStats, _debuffs, negativeEffectMultiplier);

            if (_boostDescription != null)
                _boostDescription.text = Translator.Translate(
                    $"Проклятия +{_negativeEffectIncreasePercent:0}%",
                    $"Curses +{_negativeEffectIncreasePercent:0}%",
                    $"Lanetler +%{_negativeEffectIncreasePercent:0}");

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
            _text.ForceMeshUpdate();
            int totalCharacters = _text.textInfo.characterCount;
            var wait = new WaitForSecondsRealtime(_charInterval);

            for (int i = 0; i <= totalCharacters; i++)
            {
                _text.maxVisibleCharacters = i;
                RevealIconsUpTo(i);
                yield return wait;
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
            _text.maxVisibleCharacters = int.MaxValue;
            RevealAllIcons();
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

        private void RevealIconsUpTo(int visibleCharacters)
        {
            for (int i = 0; i < _icons.Count && i < _revealAt.Count; i++)
            {
                if (visibleCharacters >= _revealAt[i])
                    RevealAnimatedIcon(_icons[i]);
            }
        }

        private void RevealAllIcons()
        {
            foreach (Image icon in _icons)
            {
                RevealAnimatedIcon(icon);
            }
        }

        private string BuildText(IReadOnlyList<DebuffRoll> debuffs)
        {
            _revealAt.Clear();
            var builder = new StringBuilder();
            builder.AppendLine(Header);

            if (debuffs != null)
            {
                foreach (DebuffRoll debuff in debuffs)
                {
                    if (debuff == null)
                        continue;

                    _revealAt.Add(builder.Length);
                    builder.AppendLine("- " + debuff.Name);
                }
            }

            return builder.ToString();
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
