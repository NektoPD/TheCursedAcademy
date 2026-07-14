using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public class CurseRevealOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _closeButton;
        [SerializeField] private string _header = "Your Curses are:";
        [SerializeField] private float _charInterval = 0.04f;

        private Coroutine _routine;

        public event Action Confirmed;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClosePressed);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClosePressed);
        }

        public void Show(IReadOnlyList<DebuffData> debuffs)
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _root.SetActive(true);
            _closeButton.gameObject.SetActive(false);
            _routine = StartCoroutine(RevealRoutine(BuildText(debuffs)));
        }

        private IEnumerator RevealRoutine(string fullText)
        {
            _text.text = fullText;
            _text.ForceMeshUpdate();
            int totalCharacters = _text.textInfo.characterCount;

            _text.maxVisibleCharacters = 0;

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
                }
            }

            return builder.ToString();
        }

        private void OnClosePressed()
        {
            _root.SetActive(false);
            Confirmed?.Invoke();
        }
    }
}
