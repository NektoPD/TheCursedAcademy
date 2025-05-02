using Items.ItemData;
using System;

namespace StatistiscSystem
{
    public class ItemStatistics
    {
        public ItemStatistics(ItemDataConfig item, float totalDamage, int level, int dps, TimeSpan timeInInventory)
        {
            Item = item;
            TotalDamage = totalDamage;
            Level = level;
            DPS = dps;
            TimeInInventory = timeInInventory;
        }

        public ItemDataConfig Item { get; private set; }

        public float TotalDamage { get; private set; }

        public int Level { get; private set; }

        public int DPS { get; private set; }

        public TimeSpan TimeInInventory { get; private set; }
    }
}