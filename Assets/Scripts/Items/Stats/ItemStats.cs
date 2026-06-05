using Items.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Items.Stats
{
    public class ItemStats
    {
        private readonly List<Stat> _stats;

        public string Item;

        public ItemStats(IReadOnlyList<Stat> templateStats)
        {
            if (templateStats == null) throw new ArgumentNullException(nameof(templateStats));
            _stats = templateStats.Select(s => new Stat(s)).ToList();
        }

        public IReadOnlyList<Stat> Stats => _stats;

        public void UpgradeStat(StatVariations variation) => GetStat(variation).LevelUp();

        public void UpgradeStats(IEnumerable<StatVariations> variations)
        {
            foreach (var v in variations)
                UpgradeStat(v);
        }

        public void SetStatStep(StatVariations variation, float step) => GetStat(variation).SetStep(step);
        public void SetStatCurrentValue(StatVariations variation, float value) => GetStat(variation).SetCurrentValue(value);
        public void SetStatNextValue(StatVariations variation, float value) => GetStat(variation).SetNextValue(value);

        private Stat GetStat(StatVariations variation) =>
            _stats.FirstOrDefault(s => s.Variation == variation)
            ?? throw new ArgumentException($"Stat {variation} {Item} not found!");
    }
}