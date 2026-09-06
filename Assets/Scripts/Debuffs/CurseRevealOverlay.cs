using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public class CurseRevealOverlay : UI.Window, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private TMP_Text _acceptButtonText;
        [SerializeField] private List<Image> _icons = new();
        [SerializeField] private string _header = "Your Curses are:";
        [SerializeField] private float _charInterval = 0.04f;
        [SerializeField] private float _acceptButtonPulseScale = 1.08f;
        [SerializeField] private float _acceptButtonPulseDuration = 0.65f;

        private readonly List<int> _revealAt = new();

        private Coroutine _routine;
        private Tween _acceptButtonPulseTween;
        private Vector3 _acceptButtonInitialScale;
        private bool _acceptButtonScaleSaved;
        private bool _isRevealing;
        private bool _choiceMade;

        public event Action<bool> Confirmed;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClosePressed);

            if (_acceptButton != null)
                _acceptButton.onClick.AddListener(OnAcceptPressed);

            SetButtonsInteractable(true);
            Opened += OnOpened;
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClosePressed);

            if (_acceptButton != null)
                _acceptButton.onClick.RemoveListener(OnAcceptPressed);

            StopAcceptButtonPulse();
            Opened -= OnOpened;
        }

        public void Show(IReadOnlyList<DebuffRoll> debuffs, float negativeEffectIncreasePercent,
            float coinBonusPercent)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _choiceMade = false;
            SetButtonsInteractable(true);
            _text.text = BuildText(debuffs);
            SetAcceptButtonText(negativeEffectIncreasePercent, coinBonusPercent);
            _text.ForceMeshUpdate();
            _text.maxVisibleCharacters = 0;
            PrepareIcons(debuffs);
            SetButtonsVisible(false);

            OpenUnscaledTime();
        }

        private void OnOpened()
        {
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
                Sprite sprite = debuffs != null && i < debuffs.Count ? debuffs[i]?.Icon : null;

                icon.sprite = sprite;
                icon.gameObject.SetActive(false);
            }
        }

        private void RevealIconsUpTo(int visibleCharacters)
        {
            for (int i = 0; i < _icons.Count && i < _revealAt.Count; i++)
            {
                if (_icons[i].gameObject.activeSelf)
                    continue;

                if (_icons[i].sprite != null && visibleCharacters >= _revealAt[i])
                    _icons[i].gameObject.SetActive(true);
            }
        }

        private void RevealAllIcons()
        {
            foreach (Image icon in _icons)
            {
                if (icon.sprite != null)
                    icon.gameObject.SetActive(true);
            }
        }

        private string BuildText(IReadOnlyList<DebuffRoll> debuffs)
        {
            _revealAt.Clear();

            var builder = new StringBuilder();
            builder.AppendLine(_header);

            if (debuffs != null)
            {
                foreach (DebuffRoll debuff in debuffs)
                {
                    if (debuff == null)
                        continue;

                    _revealAt.Add(builder.Length);
                    builder.AppendLine("- " + debuff.Name);

                    if (!string.IsNullOrEmpty(debuff.Description))
                        builder.AppendLine("   " + debuff.Description);
                }
            }

            return builder.ToString();
        }

        private void SetAcceptButtonText(float negativeEffectIncreasePercent, float coinBonusPercent)
        {
            if (_acceptButtonText == null)
                return;

            _acceptButtonText.text = Translator.Translate(
                $"Нажми, чтобы усилить отрицательные эффекты на {negativeEffectIncreasePercent:0}%\nНаграда: +{coinBonusPercent:0}% монет",
                $"Click to increase negative effects by {negativeEffectIncreasePercent:0}%\nReward: +{coinBonusPercent:0}% coins",
                $"Olumsuz etkileri %{negativeEffectIncreasePercent:0} artırmak için tıkla\nÖdül: %{coinBonusPercent:0} daha fazla jeton");
        }

        private void OnClosePressed()
        {
            CompleteChoice(false);
        }

        private void OnAcceptPressed()
        {
            CompleteChoice(true);
        }

        private void CompleteChoice(bool accepted)
        {
            if (_choiceMade)
                return;

            _choiceMade = true;
            SetButtonsInteractable(false);
            StopAcceptButtonPulse();
            CloseUnscaledTime();
            Confirmed?.Invoke(accepted);
        }

        private void SetButtonsVisible(bool visible)
        {
            _closeButton.gameObject.SetActive(visible);

            if (_acceptButton != null)
            {
                _acceptButton.gameObject.SetActive(visible);

                if (visible)
                    StartAcceptButtonPulse();
                else
                    StopAcceptButtonPulse();
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            _closeButton.interactable = interactable;

            if (_acceptButton != null)
                _acceptButton.interactable = interactable;
        }

        private void StartAcceptButtonPulse()
        {
            StopAcceptButtonPulse();

            Transform buttonTransform = _acceptButton.transform;
            _acceptButtonInitialScale = buttonTransform.localScale;
            _acceptButtonScaleSaved = true;
            _acceptButtonPulseTween = buttonTransform
                .DOScale(_acceptButtonInitialScale * _acceptButtonPulseScale, _acceptButtonPulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void StopAcceptButtonPulse()
        {
            _acceptButtonPulseTween?.Kill();
            _acceptButtonPulseTween = null;

            if (_acceptButton != null && _acceptButtonScaleSaved)
                _acceptButton.transform.localScale = _acceptButtonInitialScale;

            _acceptButtonScaleSaved = false;
        }
    }
}
