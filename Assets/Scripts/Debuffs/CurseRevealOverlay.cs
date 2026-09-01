using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        private bool _isRevealing;

        public event Action Confirmed;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClosePressed);
            Opened += OnOpened;
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClosePressed);
            Opened -= OnOpened;
        }

        public void Show(IReadOnlyList<DebuffRoll> debuffs)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _text.text = BuildText(debuffs);
            _text.ForceMeshUpdate();
            _text.maxVisibleCharacters = 0;
            PrepareIcons(debuffs);
            _closeButton.gameObject.SetActive(false);

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
            _closeButton.gameObject.SetActive(true);
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
            _closeButton.gameObject.SetActive(true);
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
                foreach (var debuff in debuffs)
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

        private void OnClosePressed()
        {
            CloseUnscaledTime();
            Confirmed?.Invoke();
        }
    }
}
