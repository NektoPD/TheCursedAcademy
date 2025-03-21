using Data;
using Utils;
using Pools;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyMover), typeof(EnemyAnimator))]
    [RequireComponent(typeof(EnemyDamageTaker), typeof(EnemyAttacker))]
    public class Enemy : MonoBehaviour, IPoolEntity
    {
        private EnemyMover _mover;
        private EnemyAnimator _view;
        private EnemyDamageTaker _damageTaker;
        private EnemyAttacker _attacker;
        private EnemyPool _pool;
        private string _name;

        public string Name => _name;

        private void Awake()
        {
            _mover = GetComponent<EnemyMover>();
            _view = GetComponent<EnemyAnimator>();
            _damageTaker = GetComponent<EnemyDamageTaker>();
            _attacker = GetComponent<EnemyAttacker>();
        }

        public void Initialize(IData<Enemy> data, EnemyPool pool)
        {
            EnemyData enemyData = data as EnemyData;

            _name = enemyData.Name;

            _damageTaker.Initialize(enemyData.Health, enemyData.ExpPointData);
            _mover.Initialize(enemyData.Speed);
            _view.Initialize(enemyData.AnimatorController);
            _attacker.Initialize(enemyData.Attacks);

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
