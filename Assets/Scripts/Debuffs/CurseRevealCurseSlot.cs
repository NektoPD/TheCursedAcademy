using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Debuffs
{
    public sealed class CurseRevealCurseSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private List<CurseRevealEffectStat> _effectStats = new();

        public IReadOnlyList<CurseRevealEffectStat> EffectStats => _effectStats;

        public int TitleCharacterCount { get; private set; }

        public void PrepareTitle(string title)
        {
            TitleCharacterCount = 0;
            if (_title == null)
                return;

            _title.text = title ?? string.Empty;
            _title.maxVisibleCharacters = int.MaxValue;
            _title.ForceMeshUpdate(true);
            TitleCharacterCount = _title.textInfo.characterCount;
            _title.maxVisibleCharacters = 0;
        }

        public void SetTitleVisibleCharacters(int count)
        {
            if (_title != null)
                _title.maxVisibleCharacters = count;
        }

        public void HideEffects()
        {
            foreach (CurseRevealEffectStat effectStat in _effectStats)
            {
                if (effectStat != null)
                    effectStat.Hide();
            }
        }

        public void RevealEffects()
        {
            foreach (CurseRevealEffectStat effectStat in _effectStats)
            {
                if (effectStat != null)
                    effectStat.Reveal();
            }
        }
    }
}
