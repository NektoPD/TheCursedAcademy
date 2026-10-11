using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

namespace Debuffs
{
    public class CurseRevealStatsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _startLabel;
        [SerializeField] private List<CurseRevealStatView> _statViews = new();

        public void Show(IReadOnlyDictionary<PerkType, float> baseStats,
            IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier)
        {
            if (_startLabel != null)
                _startLabel.text = Translator.Translate("Начать", "Start", "Başla");

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view == null)
                    continue;

                float baseline = 0f;
                if (baseStats != null)
                    baseStats.TryGetValue(view.Type, out baseline);

                view.BeginPreview(baseline);

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
    }
}
