using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Data;
using Items.BaseClass;
using Items.Stats;
using UnityEngine;
using Utils;

namespace UI.FortuneWheel
{
    public class WheelReward
    {
        public WheelRewardType Type { get; }
        public ItemVisualData Item { get; }
        public int GoldAmount { get; }
        public WheelBuffData Buff { get; }
        public string ItemUpgradeDescription { get; }

        private WheelReward(WheelRewardType type, ItemVisualData item, int goldAmount, WheelBuffData buff,
            string itemUpgradeDescription = null)
        {
            Type = type;
            Item = item;
            GoldAmount = goldAmount;
            Buff = buff;
            ItemUpgradeDescription = itemUpgradeDescription;
        }

        public static WheelReward CreateItem(ItemVisualData item) =>
            new WheelReward(WheelRewardType.Item, item, 0, null);

        public static WheelReward CreateGold(int amount) =>
            new WheelReward(WheelRewardType.Gold, null, amount, null);

        public static WheelReward CreateBuff(WheelBuffData buff) =>
            new WheelReward(WheelRewardType.Buff, null, 0, buff);

        public static WheelReward CreateItemUpgrade(Item item, int previousLevel,
            IReadOnlyList<Stat> previousStats)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(item.VisualData.Description))
                builder.AppendLine(item.VisualData.Description);

            builder.Append(Translator.Translate("Уровень", "Level", "Seviye"));
            builder.AppendLine(": " + previousLevel + " -> " + item.CurrentLevel);

            foreach (Stat stat in item.UiStats)
            {
                if (stat == null)
                    continue;

                Stat previous = null;
                foreach (Stat candidate in previousStats)
                {
                    if (candidate != null && candidate.Variation == stat.Variation)
                    {
                        previous = candidate;
                        break;
                    }
                }

                builder.Append(stat.Name + ": ");
                if (previous != null && !Mathf.Approximately(previous.CurrentValue, stat.CurrentValue))
                    builder.Append(previous.CurrentValue.ToString("0.##", CultureInfo.InvariantCulture) + " -> ");

                builder.AppendLine(stat.CurrentValue.ToString("0.##", CultureInfo.InvariantCulture));
            }

            return new WheelReward(WheelRewardType.Item, item.VisualData, 0, null, builder.ToString());
        }

        public Sprite Sprite => Type switch
        {
            WheelRewardType.Item => Item != null ? Item.Sprite : null,
            WheelRewardType.Buff => Buff != null ? Buff.Icon : null,
            _ => null
        };

        public string Label => Type switch
        {
            WheelRewardType.Item => Item != null ? Item.Name : string.Empty,
            WheelRewardType.Gold => GoldAmount.ToString(),
            WheelRewardType.Buff => Buff != null ? Buff.Name : string.Empty,
            _ => string.Empty
        };
    }
}
