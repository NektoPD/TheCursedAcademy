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
    public class CurseRevealOverlay : UI.Window, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private List<Image> _icons = new();
        [SerializeField] private string _header = "Your Curses are:";
        [SerializeField] private float _charInterval = 0.04f;

        private readonly List<int> _revealAt = new();

        private Coroutine _routine;
        private Button _acceptButton;
        private bool _isRevealing;
        private bool _choiceMade;

        public event Action<bool> Confirmed;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClosePressed);
            CreateAcceptButton();
            _acceptButton.onClick.AddListener(OnAcceptPressed);
            SetButtonsInteractable(true);
            Opened += OnOpened;
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClosePressed);

            if (_acceptButton != null)
                _acceptButton.onClick.RemoveListener(OnAcceptPressed);

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
            _text.text = BuildText(debuffs, negativeEffectIncreasePercent, coinBonusPercent);
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

        private string BuildText(IReadOnlyList<DebuffRoll> debuffs, float negativeEffectIncreasePercent,
            float coinBonusPercent)
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

            builder.AppendLine();
            builder.AppendLine(Translator.Translate(
                $"Усилить негативные эффекты на {negativeEffectIncreasePercent:0}% и получать на {coinBonusPercent:0}% больше монет?",
                $"Increase negative effects by {negativeEffectIncreasePercent:0}% and receive {coinBonusPercent:0}% more coins?",
                $"Olumsuz etkileri %{negativeEffectIncreasePercent:0} artır ve %{coinBonusPercent:0} daha fazla jeton kazan?"));

            return builder.ToString();
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
            CloseUnscaledTime();
            Confirmed?.Invoke(accepted);
        }

        private void CreateAcceptButton()
        {
            if (_acceptButton != null)
                return;

            _acceptButton = Instantiate(_closeButton, _closeButton.transform.parent);
            _acceptButton.name = "AcceptDebuffBoost";
            _acceptButton.onClick = new Button.ButtonClickedEvent();

            RectTransform declineRect = _closeButton.GetComponent<RectTransform>();
            RectTransform acceptRect = _acceptButton.GetComponent<RectTransform>();
            float offset = Mathf.Max(100f, declineRect.rect.width * 0.6f);
            declineRect.anchoredPosition += Vector2.left * offset;
            acceptRect.anchoredPosition += Vector2.right * offset;

            AddButtonLabel(_closeButton, Translator.Translate("Без усиления", "Continue", "Devam et"));
            AddButtonLabel(_acceptButton, Translator.Translate("Усилить", "Boost", "Güçlendir"));
        }

        private void AddButtonLabel(Button button, string label)
        {
            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(button.transform, false);

            RectTransform rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = labelObject.GetComponent<TextMeshProUGUI>();
            text.font = _text.font;
            text.fontSize = Mathf.Max(20f, _text.fontSize * 0.45f);
            text.color = _text.color;
            text.alignment = TextAlignmentOptions.Center;
            text.enableAutoSizing = true;
            text.fontSizeMin = 12f;
            text.fontSizeMax = text.fontSize;
            text.text = label;
        }

        private void SetButtonsVisible(bool visible)
        {
            _closeButton.gameObject.SetActive(visible);

            if (_acceptButton != null)
                _acceptButton.gameObject.SetActive(visible);
        }

        private void SetButtonsInteractable(bool interactable)
        {
            _closeButton.interactable = interactable;

            if (_acceptButton != null)
                _acceptButton.interactable = interactable;
        }
    }
}
