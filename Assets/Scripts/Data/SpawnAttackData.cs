using Data.EnemesData;
using System.Collections.Generic;
using UnityEngine;

namespace Data.AttacksData
{
    [CreateAssetMenu(fileName = "SpawnAttackData", menuName = "Attacks/SpawnAttackData ", order = 3)]
    public class SpawnAttackData : AttackData
    {
        [field: SerializeField] public int EnemyCount { get; private set; }
        [field: SerializeField] public List<EnemyData> EnemysData { get; private set; }
    }
}