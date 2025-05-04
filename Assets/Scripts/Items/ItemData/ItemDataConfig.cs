using UnityEngine;
using Enums;

namespace Items.ItemData
{
    [CreateAssetMenu(fileName = "New Item Config", menuName = "Items/Create new item")]
    public class ItemDataConfig : ScriptableObject
    {
        public string ItemNameRu;
        public string ItemNameEn;
        public string ItemNameTr;
        public float Damage;
        public float Cooldown;
        public float MaxLevel;
        public float Rarity;
        public Sprite ItemIcon;
        public string DescriptionRu;
        public string DescriptionEn;
        public string DescriptionTr;
        public Enums.ItemVariations ItemVariation;
    }
}