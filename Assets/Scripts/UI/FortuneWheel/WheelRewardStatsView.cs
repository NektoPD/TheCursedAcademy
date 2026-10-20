using System.Collections.Generic;
using Data;
using Debuffs;
using UnityEngine;

namespace UI.FortuneWheel
{
    public class WheelRewardStatsView : MonoBehaviour
    {
        [SerializeField] private List<CurseRevealStatView> _statViews = new();
        [SerializeField] private List<PerkVisualData> _visuals = new();
        [SerializeField, Min(0.01f)] private float _appearanceDuration = 0.35f;

        private readonly Dictionary<PerkType, float> _baseStats = new();
        private readonly Dictionary<PerkType, float> _multipliers = new();

        public void BeginPreview(IReadOnlyDictionary<PerkType, float> baseStats)
        {
            _baseStats.Clear();
            _multipliers.Clear();

            if (baseStats != null)
            {
                foreach (var stat in baseStats)
                    _baseStats.Add(stat.Key, stat.Value);
            }

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view == null)
                    continue;

                view.StopAppearance();
                view.gameObject.SetActive(false);
            }
        }

        public void AddBuff(WheelBuffData buff)
        {
            if (buff == null || buff.Multiplier <= 0f || buff.DurationSeconds <= 0f
                || buff.Multiplier == 1f || !_baseStats.TryGetValue(buff.Type, out float baseline))
                return;

            float multiplier = _multipliers.TryGetValue(buff.Type, out float previous) ? previous : 1f;
            multiplier *= buff.Multiplier;
            _multipliers[buff.Type] = multiplier;

            foreach (CurseRevealStatView view in _statViews)
            {
                if (view == null || view.Type != buff.Type)
                    continue;

                view.StopAppearance();
                view.BeginPreview(baseline, FindStatIcon(view.Type));
                view.AddEffect(multiplier);
                view.CompletePreview();
                view.gameObject.SetActive(true);
                view.PrepareAppearance();
                view.PlayAppearance(0f, Mathf.Max(0.01f, _appearanceDuration));
            }
        }

        public void StopAppearance()
        {
            foreach (CurseRevealStatView view in _statViews)
            {
                if (view != null)
                    view.StopAppearance();
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

        private void OnDisable()
        {
            StopAppearance();
        }
    }
}
