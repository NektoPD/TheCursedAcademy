using Data.ProjectilesData;
using UnityEngine;

namespace Data.AttacksData
{
    [CreateAssetMenu(fileName = "RangeAttackData", menuName = "Attacks/RangeAttackData ", order = 2)]
    public class RangeAttackData : AttackData
    {
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public int CountProjectiles { get; private set; }
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
    }
}
