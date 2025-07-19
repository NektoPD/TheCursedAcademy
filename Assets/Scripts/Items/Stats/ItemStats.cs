using Data;
using Items.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Items.Stats
{
    public class ItemStats
    {
        private readonly ItemVisualData _visualData;

        public ItemStats(ItemVisualData visualData)
        {
            _visualData = visualData;
        }

        public void UpgradeStat(StatVariations variation) => GetStat(variation).LevelUp();

        public void UpgradeStats(IEnumerable<StatVariations> variations) 
        { 
            foreach (StatVariations stat in variations)
                UpgradeStat(stat);
        }

        public void SetStatStep(StatVariations variation, float step) => GetStat(variation).SetStep(step);

        public void SetStatCurrentValue(StatVariations variation, float value) => GetStat(variation).SetCurrentValue(value);

        public void SetStatNextValue(StatVariations variation, float value) => GetStat(variation).SetNextValue(value);

        private Stat GetStat(StatVariations variation) => _visualData.Stats.FirstOrDefault(stat => stat.Variation == variation) ?? throw new ArgumentException($"Stat {variation} not found!");
    }
}