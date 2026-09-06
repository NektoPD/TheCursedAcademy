using Data;
using UnityEngine;

namespace UI.FortuneWheel
{
    public class WheelReward
    {
        public WheelRewardType Type { get; }
        public ItemVisualData Item { get; }
        public int GoldAmount { get; }
        public WheelBuffData Buff { get; }

        private WheelReward(WheelRewardType type, ItemVisualData item, int goldAmount, WheelBuffData buff)
        {
            Type = type;
            Item = item;
            GoldAmount = goldAmount;
            Buff = buff;
        }

        public static WheelReward CreateItem(ItemVisualData item) =>
            new WheelReward(WheelRewardType.Item, item, 0, null);

        public static WheelReward CreateGold(int amount) =>
            new WheelReward(WheelRewardType.Gold, null, amount, null);

        public static WheelReward CreateBuff(WheelBuffData buff) =>
            new WheelReward(WheelRewardType.Buff, null, 0, buff);

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
