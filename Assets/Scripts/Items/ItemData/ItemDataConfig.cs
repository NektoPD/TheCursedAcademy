using UnityEngine;

namespace Items.ItemData
{
    public class ItemDataConfig : ScriptableObject
    {
        public string ItemName;
        public float Damage;
        public float Cooldown;
        public float MaxLevel;
        public float Rarity;
        public Sprite ItemIcon;
        public string Description;
    }
}