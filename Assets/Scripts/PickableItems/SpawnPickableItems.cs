using Pools;
using Pools.FromPrefab;
using UnityEngine;
using Zenject;

namespace PickableItems
{
    public class SpawnPickableItems : MonoBehaviour
    {
        private readonly float _minChance = 0f;
        private readonly float _maxChance = 1f;

        [SerializeField][Range(0, 1)] private float _magnetSpawnChance = 0.05f;
        [SerializeField][Range(0, 1)] private float _healSpawnChance = 0.2f;
        [SerializeField] private int _maxHealValue = 10;

        private EnemyPool _enemyPool;
        private HealPool _healPool;
        private MagnetPool _magnetPool;

        [Inject]
        private void Construct(EnemyPool enemyPool, HealPool healPool, MagnetPool magnetPool)
        {
            _enemyPool = enemyPool;
            _healPool = healPool;
            _magnetPool = magnetPool;
        }

        private void OnEnable()
        {
            _enemyPool.EnemyReturned += OnEnemyReturned;
        }

        private void OnDisable()
        {
            _enemyPool.EnemyReturned -= OnEnemyReturned;
        }

        private void OnEnemyReturned(Transform transform)
        {
            Transform item = null;

            if (IsWin(_healSpawnChance))
                item = _healPool.Get(Random.Range(1, _maxHealValue)).transform;

            if (IsWin(_magnetSpawnChance))
                item = _magnetPool.Get(0).transform;

            if(item != null) 
                item.transform.position = transform.position;
        }

        private bool IsWin(float chance) => Random.Range(_minChance, _maxChance) <= chance;
    }
}