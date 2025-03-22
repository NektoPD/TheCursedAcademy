using EnemyLogic;
using UnityEngine;
using System.Collections.Generic;

namespace Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemys/EnemyData ", order = 1)]
    public class EnemyData : ScriptableObject, IData<Enemy>
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private float _health;
        [SerializeField] private float _speed;
        [SerializeField] private List<AttackData> _attacks;
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [SerializeField] private ExpPointData _expPointData;
        [SerializeField] private Enemy _prefab;

        public int Id => _id;

        public string Name => _name;

        public float Health => _health;

        public float Speed => _speed;

        public IReadOnlyList<AttackData> Attacks => _attacks;

        public RuntimeAnimatorController AnimatorController => _animatorController;

        public ExpPointData ExpPointData => _expPointData;

        public Enemy Prefab => _prefab;
    }
}
