using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public class CurseRevealOverlay : UI.Window
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private string _header = "Your Curses are:";
        [SerializeField] private float _charInterval = 0.04f;

        private Coroutine _routine;

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

        public void Show(IReadOnlyList<DebuffData> debuffs)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _text.text = BuildText(debuffs);
            _text.ForceMeshUpdate();
            _text.maxVisibleCharacters = 0;
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
            _text.ForceMeshUpdate();
            int totalCharacters = _text.textInfo.characterCount;

            var wait = new WaitForSecondsRealtime(_charInterval);

            for (int i = 0; i <= totalCharacters; i++)
            {
                _text.maxVisibleCharacters = i;
                yield return wait;
            }

            _closeButton.gameObject.SetActive(true);
            _routine = null;
        }

        private string BuildText(IReadOnlyList<DebuffData> debuffs)
        {
            var builder = new StringBuilder();
            builder.AppendLine(_header);

            if (debuffs != null)
            {
                foreach (var debuff in debuffs)
                {
                    if (debuff == null)
                        continue;

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
