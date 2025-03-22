using Data.EnemesData;
using EnemyLogic;
using Pools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Zenject;

namespace Difficulties
{
    [RequireComponent(typeof(TimeTracker), typeof(OffscreenPositionGenerator))]
    public class Difficulty : MonoBehaviour
    {
        [SerializeField] private float _cooldown = 1;
        [SerializeField] private int _maxEnemy = 100;

        private EnemyPool _enemyPool;
        private TimeTracker _timeTracker;
        private OffscreenPositionGenerator _positionGenerator;
        private List<EnemyData> _enemyDataList;
        private List<int> _enemyIds;
        private Coroutine _coroutine;

        private bool _canSpawn = true;

        private readonly int _minIdBoses = 100;

        [Inject]
        public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList)
        {
            _enemyPool = enemyPool;
            _enemyDataList = enemyDataList;
        }

        private void Awake()
        {
            _timeTracker = GetComponent<TimeTracker>();
            _positionGenerator = GetComponent<OffscreenPositionGenerator>();
        }

        private void OnEnable()
        {
            _timeTracker.TimeComed += SetIds;
        }

        private void OnDisable()
        {
            _timeTracker.TimeComed -= SetIds;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private void Update()
        {
            if (_canSpawn && _enemyPool.Active < _maxEnemy)
            {
                _canSpawn = false;

                Enemy enemy = GetRandomEnemy();

                if (enemy == null)
                    return;

                enemy.transform.position = _positionGenerator.GetRandomPositionOutsideCamera();

                _coroutine = StartCoroutine(Cooldown());
            }
        }

        private void SetIds(List<int> ids) => _enemyIds = ids;

        private Enemy GetRandomEnemy()
        {
            if (_enemyIds == null || _enemyIds.Count == 0)
                return null;

            int id = _enemyIds[Random.Range(0, _enemyIds.Count)];

            if (id >= _minIdBoses)
                _enemyIds.Remove(id);

            var data = _enemyDataList.First(enemy => enemy.Id == id);

            return _enemyPool.Get(data);
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(_cooldown);

            _canSpawn = true;
        }
    }
}