using Data.EnemesData;
using Difficulties.TimeTrackers;
using Difficulties.TimeTrackers.TimeDatas;
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
    public class EventStarter : MonoBehaviour
    {
        private const string Key = nameof(GroupEnemysEventData);

        [SerializeField] private float spawnRadius = 1f;

        private EnemyPool _enemyPool;
        private EventTimeTracker _timeTracker;
        private List<EnemyData> _enemyDataList;

        private Coroutine _coroutine;

        [Inject]
        public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList)
        {
            _enemyPool = enemyPool;
            _enemyDataList = enemyDataList;
        }

        private void Awake()
        {
            _timeTracker = new EventTimeTracker();
        }

        private void Start()
        {
            _timeTracker.Start();
        }

        private void OnEnable()
        {
            _timeTracker.TimeComed += SetData;
        }

        private void OnDisable()
        {
            _timeTracker.TimeComed -= SetData;
        }

        private void SetData(GroupEnemysEventData data)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(Countdown(data));
        }

        private IEnumerator Countdown(GroupEnemysEventData data)
        {
            var wait = new WaitForSeconds(data.Cooldown);

            while (enabled)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    var position = OffscreenPositionGenerator.GetRandomPositionOutsideCamera();
                    Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
                    Vector2 spawnPosition = new(position.x + randomPoint.x, position.y + randomPoint.y);
                    var enemyId = data.EnemyIds[Random.Range(0, data.EnemyIds.Count)];
                    var enemy = _enemyPool.Get(_enemyDataList.First(enemy => enemy.Id == enemyId));
                    enemy.transform.position = spawnPosition;
                }

                yield return wait;
            }
        }
    }
}