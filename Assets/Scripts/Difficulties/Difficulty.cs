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
    public class Difficulty : MonoBehaviour
    {
        private const string DataKey = nameof(DifficultyData);
        private const string CooldownKey = "DifficultyCooldown";
        private const string MaxEnemyKey = "DifficultyMaxEnemy";

        [SerializeField] private float _offset = 0.1f;
        [SerializeField] private float _cooldownChangeForWave = 0.1f;

        private EnemyPool _enemyPool;
        private TimeTracker<DifficultyData> _timeTracker;
        private List<EnemyData> _enemyDataList;
        private List<int> _enemyIds;
        private Coroutine _coroutine;
        private float _cooldown;
        private int _maxEnemy;

        private bool _canSpawn = true;
        private bool _isLastTime = false;

        private readonly int _minIdBoses = 100;

        [Inject]
        public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList)
        {
            _enemyPool = enemyPool;
            _enemyDataList = enemyDataList;
        }

        private void Awake()
        {
            _timeTracker = new TimeTracker<DifficultyData>(DataKey);

            if(PlayerPrefs.HasKey(CooldownKey))
                _cooldown = PlayerPrefs.GetFloat(CooldownKey);

            if(PlayerPrefs.HasKey(MaxEnemyKey))
                _maxEnemy = PlayerPrefs.GetInt(MaxEnemyKey);
        }

        private void Start()
        {
            _timeTracker.Start();
        }

        private void OnEnable()
        {
            _timeTracker.TimeComed += NewWaveEnemysConfig;
            _timeTracker.LastTimeComed += SetLastTime;
        }

        private void OnDisable()
        {
            _timeTracker.TimeComed -= NewWaveEnemysConfig;
            _timeTracker.LastTimeComed -= SetLastTime;

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

                enemy.transform.position = OffscreenPositionGenerator.GetRandomPositionOutsideCamera(_offset);

                _coroutine = StartCoroutine(Cooldown());
            }
        }

        private void NewWaveEnemysConfig(DifficultyData data)
        {
            _enemyIds = data.EnemyIds.ToList();
            _cooldown = Mathf.Clamp01(_cooldown -= _cooldownChangeForWave);
        }

        private void SetLastTime() => _isLastTime = true;

        private Enemy GetRandomEnemy()
        {
            if (_enemyIds == null || _enemyIds.Count == 0)
                return null;

            int id = _enemyIds[Random.Range(0, _enemyIds.Count)];

            if (id >= _minIdBoses && _isLastTime == false)
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