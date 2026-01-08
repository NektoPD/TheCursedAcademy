using Items.Enums;
using UnityEngine;

namespace Items.ItemData
{
    [CreateAssetMenu(fileName = "New Item Config", menuName = "Items/Create new item")]
    public class ItemDataConfig : ScriptableObject
    {
        public float Damage;
        public float Cooldown;
        public float MaxLevel;
        public float Rarity;
        public Sprite ItemIcon;
        public Enums.ItemVariations ItemVariation;

        public bool TryGetStatValue(StatVariations variation, out float value)
        {
            value = 0f;

            switch (variation)
            {
                case StatVariations.Damage:
                    value = Damage;
                    return true;

                case StatVariations.AttackSpeed:
                    value = Cooldown > 0 ? 1f / Cooldown : 0f;
                    return true;

                default:
                    return false;
            }
        }
    }
}