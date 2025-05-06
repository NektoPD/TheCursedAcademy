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
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private float _health;
        [SerializeField] private float _speed;
        [SerializeField] private float _immuneTime;
        [SerializeField] private int _money;
        [SerializeField] private int _moneyDropChancePerProcent;
        [SerializeField] private List<AttackData> _attacks;
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [SerializeField] private ExpPointData _expPointData;
        [SerializeField] private Enemy _prefab;

        public int Id => _id;

        public string Name => _name;

        public float Health => _health;

        public float Speed => _speed;

        public float ImmuneTime => _immuneTime;

        public int Money => _money;

        public int MoneyDropChancePerProcent => _moneyDropChancePerProcent;

        public IReadOnlyList<AttackData> Attacks => _attacks;

        public RuntimeAnimatorController AnimatorController => _animatorController;

        public ExpPointData ExpPointData => _expPointData;

        public Enemy Prefab => _prefab;
    }
}
