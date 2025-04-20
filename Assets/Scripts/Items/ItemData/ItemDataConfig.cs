using UnityEngine;

namespace Items.ItemData
{
    [CreateAssetMenu(fileName = "New Item Config", menuName = "Items/Create new item")]
    public class ItemDataConfig : ScriptableObject
    {
        public string ItemName;
        public float Damage;
        public float Cooldown;
        public float MaxLevel;
        public float Rarity;
        public Sprite ItemIcon;
        public string Description;
        public Enums.ItemVariations ItemVariation;
    }
}