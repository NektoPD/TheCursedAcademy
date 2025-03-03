using UnityEngine;

namespace Items.ItemData
{
    public class ItemDataConfig : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float MaxLevel { get; private set; }
        [field: SerializeField] public float Rarity { get; private set; }
        [field: SerializeField] public Sprite ItemIcon { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
    }
}