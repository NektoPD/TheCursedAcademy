using UnityEngine;
using Enums;

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
    }
}