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

        // IMPORTANT: боссы у теб€ 1000+, поэтому порог должен быть 1000 (а не 100)
        private const int BossMinId = 1000;

        [Header("Spawn")]
        [SerializeField] private float _offset = 0.1f;

        [Header("Tuning")]
        [SerializeField] private float _cooldownChangeForWave = 0.1f;

        [Header("Fallback defaults (if PlayerPrefs empty)")]
        [SerializeField] private float _defaultCooldown = 1f;
        [SerializeField] private int _defaultMaxEnemy = 100;

        private EnemyPool _enemyPool;
        private TimeTracker<DifficultyData> _timeTracker;
        private List<EnemyData> _enemyDataList;

        // ids текущей волны
        private readonly List<int> _regularIds = new();
        private readonly Queue<int> _bossSpawnQueue = new();

        private Coroutine _cooldownRoutine;
        private Coroutine _bossRoutine;

        private float _cooldown;
        private int _maxEnemy;

        private bool _canSpawn = true;

        [Inject]
        public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList)
        {
            _enemyPool = enemyPool;
            _enemyDataList = enemyDataList;
        }

        private void Awake()
        {
            _timeTracker = new TimeTracker<DifficultyData>(DataKey);

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

        private void Start()
        {
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
            // обычный спавн (не босс)
            if (!_canSpawn)
                return;

            if (_enemyPool.Active >= _maxEnemy)
                return;

            var enemy = GetRandomRegularEnemy();
            if (enemy == null)
            {
                // Ќ» ј ќ√ќ "залипани€": если никого не нашли, просто уйдЄм в кулдаун
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
            // 1) обновл€ем списки волны
            _regularIds.Clear();
            _bossSpawnQueue.Clear();

            // делим ids на обычных и боссов
            foreach (var id in data.EnemyIds)
            {
                if (id >= BossMinId)
                    _bossSpawnQueue.Enqueue(id);
                else
                    _regularIds.Add(id);
            }

            // 2) кулдаун усложнени€
            _cooldown = Mathf.Clamp01(_cooldown - _cooldownChangeForWave);

            // 3) гарантированно запускаем спавн боссов этой волны
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
                // если данных нет Ч убираем плохой id, чтобы не упиратьс€ в него посто€нно
                _regularIds.Remove(id);
                return null;
            }

            return _enemyPool.Get(data);
        }

        private IEnumerator SpawnBossesGuaranteed()
        {
            while (_bossSpawnQueue.Count > 0)
            {
                // ждЄм, пока будет место под врага (учитываем общий лимит)
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

                // чтобы боссы не спавнились "в одну точку/в один кадр", можно дать микро-паузу
                // (можешь убрать или поставить 0)
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
