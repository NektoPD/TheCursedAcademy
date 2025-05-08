using UnityEngine;

namespace Data.AttacksData
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Attacks/MeleeAttackData ", order = 1)]
    public class MeleeAttackData : AttackData
    {
        [field: SerializeField] public float Damage { get; private set; }
    }
}
