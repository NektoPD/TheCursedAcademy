using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SpawnAttackData", menuName = "Attacks/SpawnAttackData ", order = 3)]
    public class SpawnAttackData : AttackData
    {
        [SerializeField] private int _countEnemy;
        [SerializeField] private List<EnemyData> _enemysData;

        public int EnemyCount => _countEnemy;

        public IReadOnlyList<EnemyData> EnemysData => _enemysData;
    }
}