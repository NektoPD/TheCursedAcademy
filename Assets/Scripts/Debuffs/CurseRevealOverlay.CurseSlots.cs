using System.Collections.Generic;
using UnityEngine;

namespace Debuffs
{
    public partial class CurseRevealOverlay
    {
        [SerializeField] private List<CurseRevealCurseSlot> _curseSlots = new();

        private void PrepareCurseEffectStats(IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier)
        {
            HideCurseEffectStats();

            if (_statsView == null || debuffs == null)
                return;

            for (int curseIndex = 0; curseIndex < _curseSlots.Count; curseIndex++)
            {
                CurseRevealCurseSlot curseSlot = _curseSlots[curseIndex];
                if (curseSlot == null || curseIndex >= debuffs.Count || debuffs[curseIndex] == null)
                    continue;

                List<DebuffModifier> modifiers = GetAggregateModifiers(
                    debuffs[curseIndex], negativeEffectMultiplier);
                IReadOnlyList<CurseRevealEffectStat> effectStats = curseSlot.EffectStats;

                for (int effectIndex = 0; effectIndex < effectStats.Count; effectIndex++)
                {
                    CurseRevealEffectStat effectStat = effectStats[effectIndex];
                    if (effectStat == null)
                        continue;

                    if (effectIndex >= modifiers.Count)
                    {
                        effectStat.Hide();
                        continue;
                    }

                    DebuffModifier modifier = modifiers[effectIndex];
                    effectStat.Prepare(_statsView.GetStatIcon(modifier.Type),
                        modifier.Multiplier, modifier.Type);
                }
            }
        }

        private void PrepareCurseTitles(IReadOnlyList<DebuffRoll> debuffs)
        {
            for (int i = 0; i < _curseSlots.Count; i++)
            {
                if (_curseSlots[i] == null)
                    continue;

                string title = debuffs != null && i < debuffs.Count && debuffs[i] != null
                    ? debuffs[i].Name
                    : string.Empty;
                _curseSlots[i].PrepareTitle(title);
            }
        }

        private void RevealAllCurseTitles()
        {
            foreach (CurseRevealCurseSlot slot in _curseSlots)
            {
                if (slot != null)
                    slot.SetTitleVisibleCharacters(int.MaxValue);
            }
        }

        private void RevealAllCurseEffectStats()
        {
            foreach (CurseRevealCurseSlot curseSlot in _curseSlots)
            {
                if (curseSlot != null)
                    curseSlot.RevealEffects();
            }
        }

        private void HideCurseEffectStats()
        {
            foreach (CurseRevealCurseSlot curseSlot in _curseSlots)
            {
                if (curseSlot != null)
                    curseSlot.HideEffects();
            }
        }

        private List<DebuffModifier> GetAggregateModifiers(
            DebuffRoll debuff, float negativeEffectMultiplier)
        {
            Dictionary<PerkType, float> values = new();

            foreach (DebuffModifier modifier in debuff.GetModifiers(negativeEffectMultiplier))
            {
                if (!values.ContainsKey(modifier.Type))
                    values.Add(modifier.Type, 1f);

                values[modifier.Type] *= modifier.Multiplier;
            }

            var result = new List<DebuffModifier>();
            foreach (KeyValuePair<PerkType, float> value in values)
            {
                if (!Mathf.Approximately(value.Value, 1f)
                    && _statsView.GetStatIcon(value.Key) != null)
                    result.Add(new DebuffModifier(value.Key, value.Value));
            }

            return result;
        }
    }
}
