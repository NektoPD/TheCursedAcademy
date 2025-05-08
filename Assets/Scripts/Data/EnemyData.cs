using Data.AttacksData;
using Data.ExpPointsData;
using EnemyLogic;
using System.Collections.Generic;
using UnityEngine;

namespace Data.EnemesData
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemys/EnemyData ", order = 1)]
    public class EnemyData : ScriptableObject, IData<Enemy>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float Health { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float ImmuneTime { get; private set; }
        [field: SerializeField] public int Money { get; private set; }
        [field: SerializeField] public int MoneyDropChancePerProcent { get; private set; }
        [field: SerializeField] public List<AttackData> Attacks { get; private set; }
        [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public ExpPointData ExpPointData { get; private set; }
        [field: SerializeField] public Enemy Prefab { get; private set; }
    }
}
