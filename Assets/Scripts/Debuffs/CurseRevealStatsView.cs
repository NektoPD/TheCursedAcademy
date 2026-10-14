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
        [SerializeField, Min(0f)] private float _appearanceStagger = 0.12f;
        [SerializeField, Min(0.01f)] private float _appearanceDuration = 0.35f;

        private bool _appearancePrepared;

        public void PrepareAppearance()
        {
            StopAppearance();

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view != null)
                    view.PrepareAppearance();
            }

            _appearancePrepared = true;
        }

        public void PlayAppearance()
        {
            if (!_appearancePrepared)
                return;

            _appearancePrepared = false;
            int index = 0;
            float stagger = Mathf.Max(0f, _appearanceStagger);
            float duration = Mathf.Max(0.01f, _appearanceDuration);

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view == null)
                    continue;

                view.PlayAppearance(index * stagger, duration);
                index++;
            }
        }

        public void StopAppearance()
        {
            _appearancePrepared = false;

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view != null)
                    view.StopAppearance();
            }
        }

        private void OnDisable()
        {
            StopAppearance();
        }

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
                                view.AddEffect(modifier.Multiplier);
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
