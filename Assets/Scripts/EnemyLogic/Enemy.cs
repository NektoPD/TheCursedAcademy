using Data;
using Data.EnemesData;
using Pools;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyMover), typeof(EnemyAnimator), typeof(EnemyEjector))]
    [RequireComponent(typeof(EnemyDamageTaker), typeof(EnemyAttacker))]
    public class Enemy : MonoBehaviour, IPoolEntity
    {
        private EnemyMover _mover;
        private EnemyAnimator _animator;
        private EnemyDamageTaker _damageTaker;
        private EnemyAttacker _attacker;
        private EnemyPool _pool;
        private EnemyEjector _ejector;
        private string _name;

        public string Name => _name;

        private void Awake()
        {
            _mover = GetComponent<EnemyMover>();
            _animator = GetComponent<EnemyAnimator>();
            _damageTaker = GetComponent<EnemyDamageTaker>();
            _attacker = GetComponent<EnemyAttacker>();
            _ejector = GetComponent<EnemyEjector>();
        }

        public void Initialize(IData<Enemy> data, EnemyPool pool)
        {
            EnemyData enemyData = data as EnemyData;

            _name = enemyData.Name;
            _animator.Initialize(enemyData.AnimatorController);
            _damageTaker.Initialize(enemyData.Health, enemyData.ImmuneTime);
            _mover.Initialize(enemyData.Speed);
            _attacker.Initialize(enemyData.Attacks);
            _ejector.Initialize(enemyData.ExpPointData, enemyData.Money);

            _pool = pool;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}
