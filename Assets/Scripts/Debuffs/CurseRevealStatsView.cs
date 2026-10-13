using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using Utils;

namespace Debuffs
{
    public class CurseRevealStatsView : MonoBehaviour
    {
        [SerializeField] private List<CurseRevealStatView> _statViews = new();
        [SerializeField] private List<PerkVisualData> _visuals = new();

        public void Show(IReadOnlyDictionary<PerkType, float> baseStats,
            IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier)
        {
            foreach (CurseRevealStatView view in _statViews)
            {
                if (view == null)
                    continue;

                float baseline = 0f;
                if (baseStats != null)
                    baseStats.TryGetValue(view.Type, out baseline);

                view.BeginPreview(baseline, FindStatIcon(view.Type));

                if (debuffs != null)
                {
                    foreach (DebuffRoll roll in debuffs)
                    {
                        if (roll == null)
                            continue;

                        foreach (DebuffModifier modifier in roll.GetModifiers(negativeEffectMultiplier))
                        {
                            if (modifier.Type == view.Type && modifier.Multiplier != 1f)
                                view.AddEffect(roll.Icon, modifier.Multiplier);
                        }
                    }
                }

                view.CompletePreview();
            }
        }

        private Sprite FindStatIcon(PerkType type)
        {
            foreach (PerkVisualData visual in _visuals)
            {
                if (visual != null && visual.Type == type)
                    return visual.Sprite;
            }

            return null;
        }
    }
}
