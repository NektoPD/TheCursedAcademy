using Items.BaseClass;
using UnityEngine;

namespace CharacterLogic.Data
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Character/Create new character")]
    public class CharacterData : ScriptableObject
    {
        public enum CharacterType
        {
            Girl1,
            Girl2,
            Girl3
        }

        [field: SerializeField] public float AttackPower { get; private set; }
        [field: SerializeField] public float Armor { get; private set; }
        [field: SerializeField] public float Hp { get; private set; }
        [field: SerializeField] public float HpRegenerationSpeed { get; private set; }
        [field: SerializeField] public float AttackRegenerationSpeed { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public CharacterType Type { get; private set; }
        [field: SerializeField] public AnimatorOverrideController AnimatorController { get; private set; }
        [field: SerializeField] public Item StartItem { get; private set; }
    }
}