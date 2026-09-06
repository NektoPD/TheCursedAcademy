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

        private const int BossMinId = 1000;

        [Header("Spawn")]
        [SerializeField] private float _offset = 0.1f;

        [Header("Tuning")]
        [SerializeField] private float _cooldownChangeForWave = 0.1f;

        [Header("XP tuning (per wave)")]
        [SerializeField] private float _xpBaseMultiplier = 1f;
        [SerializeField] private float _xpGrowthPerWave = 1.15f;

        [Header("Fallback defaults (if PlayerPrefs empty)")]
        [SerializeField] private float _defaultCooldown = 1f;
        [SerializeField] private int _defaultMaxEnemy = 100;

        private EnemyPool _enemyPool;
        private TimeTracker<DifficultyData> _timeTracker;
        private List<EnemyData> _enemyDataList;
        private XpWaveScaler _xpWaveScaler;

        private readonly List<int> _regularIds = new();
        private readonly Queue<int> _bossSpawnQueue = new();

        private Coroutine _cooldownRoutine;
        private Coroutine _bossRoutine;

        private float _cooldown;
        private int _maxEnemy;

        private bool _canSpawn = true;
        private bool _isStarted = false;
        private int _waveIndex = 0;

        [Inject]
        public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList, XpWaveScaler xpWaveScaler)
        {
            _enemyPool = enemyPool;
            _enemyDataList = enemyDataList;
            _xpWaveScaler = xpWaveScaler;
        }

        private void Awake()
        {
            _timeTracker = new TimeTracker<DifficultyData>(DataKey);

            _waveIndex = 0;
            if (_xpWaveScaler != null)
            {
                _xpWaveScaler.Configure(_xpBaseMultiplier, _xpGrowthPerWave);
                _xpWaveScaler.SetWaveIndex(_waveIndex);
            }

            _cooldown = PlayerPrefs.HasKey(CooldownKey)
                ? PlayerPrefs.GetFloat(CooldownKey)
                : _defaultCooldown;

            _maxEnemy = PlayerPrefs.HasKey(MaxEnemyKey)
                ? PlayerPrefs.GetInt(MaxEnemyKey)
                : _defaultMaxEnemy;
        }

        private void OnEnable()
        {
            _timeTracker.TimeComed += OnWaveChanged;
        }

        [Header("Game start")]
        [SerializeField] private bool _waitForGameStart = true;

        private void Start()
        {
            if (!_waitForGameStart)
                StartSpawning();
        }

        public void StartSpawning()
        {
            if (_isStarted)
                return;

            _isStarted = true;
            _timeTracker.Start();
        }

        private void OnDisable()
        {
            _timeTracker.TimeComed -= OnWaveChanged;

            if (_cooldownRoutine != null)
                StopCoroutine(_cooldownRoutine);

            if (_bossRoutine != null)
                StopCoroutine(_bossRoutine);
        }

        private void Update()
        {
            if (!_isStarted)
                return;

            if (!_canSpawn)
                return;

            if (_enemyPool.Active >= _maxEnemy)
                return;

            var enemy = GetRandomRegularEnemy();
            if (enemy == null)
            {
                _canSpawn = false;
                _cooldownRoutine = StartCoroutine(Cooldown());
                return;
            }

            _canSpawn = false;

            enemy.transform.position =
                OffscreenPositionGenerator.GetRandomPositionOutsideCamera(_offset);

            _cooldownRoutine = StartCoroutine(Cooldown());
        }

        private void OnWaveChanged(DifficultyData data)
        {
            if (_xpWaveScaler != null)
                _xpWaveScaler.SetWaveIndex(_waveIndex);
            _waveIndex++;

            _regularIds.Clear();
            _bossSpawnQueue.Clear();

            foreach (var id in data.EnemyIds)
            {
                if (id >= BossMinId)
                    _bossSpawnQueue.Enqueue(id);
                else
                    _regularIds.Add(id);
            }

            _cooldown = Mathf.Clamp01(_cooldown - _cooldownChangeForWave);

            if (_bossRoutine != null)
                StopCoroutine(_bossRoutine);

            if (_bossSpawnQueue.Count > 0)
                _bossRoutine = StartCoroutine(SpawnBossesGuaranteed());
        }

        private Enemy GetRandomRegularEnemy()
        {
            if (_regularIds.Count == 0)
                return null;

            int id = _regularIds[Random.Range(0, _regularIds.Count)];

            var data = _enemyDataList.FirstOrDefault(e => e.Id == id);
            if (data == null)
            {
                _regularIds.Remove(id);
                return null;
            }

            return _enemyPool.Get(data);
        }

        private IEnumerator SpawnBossesGuaranteed()
        {
            while (_bossSpawnQueue.Count > 0)
            {
                while (_enemyPool.Active >= _maxEnemy)
                    yield return null;

                int bossId = _bossSpawnQueue.Dequeue();

                var data = _enemyDataList.FirstOrDefault(e => e.Id == bossId);
                if (data == null)
                {
                    Debug.LogWarning($"[Difficulty] Boss id {bossId} not found in EnemyDataList");
                    continue;
                }

                var enemy = _enemyPool.Get(data);
                if (enemy == null)
                {
                    Debug.LogWarning($"[Difficulty] EnemyPool returned null for boss id {bossId}");
                    continue;
                }

                enemy.transform.position =
                    OffscreenPositionGenerator.GetRandomPositionOutsideCamera(_offset);

                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(_cooldown);
            _canSpawn = true;
        }
    }
}
