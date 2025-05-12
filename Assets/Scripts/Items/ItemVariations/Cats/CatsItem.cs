using System;
using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Items.ItemVariations.Cats
{
    public class CatsItem : Item
    {
        [SerializeField] private CatsProjectile _catPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _catLifetime = 10f;
        [SerializeField] private float _maxDistanceFromPlayer = 3f;
        [SerializeField] private float _catMovementSpeed = 3f;
        [SerializeField] private int _catsPerSpawn = 3;
        [SerializeField] private float _detectionRadius = 5f;

        [Header("Particle Effect")] [SerializeField]
        private ParticleSystem _catDisappearEffectPrefab;

        [SerializeField] private int _effectPoolSize = 10;

        [Header("Level Up Settings")] [SerializeField]
        private int _baseCatsPerSpawn = 3;

        [SerializeField] private int _catsPerLevelDiv = 2;
        [SerializeField] private float _baseDetectionRadius = 5f;
        [SerializeField] private float _detectionRadiusPerLevel = 0.5f;
        [SerializeField] private float _maxDetectionRadius = 10f;

        [Header("Pool Settings")] [SerializeField]
        private int _poolDefaultCapacity = 10;

        [SerializeField] private int _poolMaxSize = 50;

        private ObjectPool<CatsProjectile> _catPool;
        private ObjectPool<ParticleSystem> _effectPool;
        private List<CatsProjectile> _activeCats = new List<CatsProjectile>();
        private bool _catsActive = false;
        private Coroutine _respawnCoroutine;
        private Transform _transform;

        private Dictionary<IDamageable, CatsProjectile>
            _targetedEnemies = new Dictionary<IDamageable, CatsProjectile>();

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _catPool = new ObjectPool<CatsProjectile>(
                createFunc: CreateCat,
                actionOnGet: OnGetCatFromPool,
                actionOnRelease: OnReleaseCatToPool,
                actionOnDestroy: OnDestroyCatObject,
                collectionCheck: false,
                defaultCapacity: _poolDefaultCapacity,
                maxSize: _poolMaxSize
            );

            if (_catDisappearEffectPrefab != null)
            {
                _effectPool = new ObjectPool<ParticleSystem>(
                    createFunc: CreateEffect,
                    actionOnGet: OnGetEffectFromPool,
                    actionOnRelease: OnReleaseEffectToPool,
                    actionOnDestroy: OnDestroyEffectObject,
                    collectionCheck: false,
                    defaultCapacity: _effectPoolSize,
                    maxSize: _effectPoolSize * 2
                );

                List<ParticleSystem> tempEffects = new List<ParticleSystem>(_effectPoolSize);
                for (int i = 0; i < _effectPoolSize; i++)
                {
                    ParticleSystem effect = _effectPool.Get();
                    tempEffects.Add(effect);
                }

                foreach (var effect in tempEffects)
                {
                    _effectPool.Release(effect);
                }
            }
        }

        private ParticleSystem CreateEffect()
        {
            ParticleSystem effect = Instantiate(_catDisappearEffectPrefab);
            effect.gameObject.SetActive(false);
            return effect;
        }

        private void OnGetEffectFromPool(ParticleSystem effect)
        {
            effect.gameObject.SetActive(true);
        }

        private void OnReleaseEffectToPool(ParticleSystem effect)
        {
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyEffectObject(ParticleSystem effect)
        {
            Destroy(effect.gameObject);
        }

        private CatsProjectile CreateCat()
        {
            CatsProjectile cat = Instantiate(_catPrefab, _spawnPoint.position, Quaternion.identity);

            if (_catDisappearEffectPrefab != null)
            {
                ParticleSystem effect = _effectPool.Get();

                _effectPool.Release(effect);
            }

            cat.SetPool(_catPool);
            cat.SetController(this);
            return cat;
        }

        private void OnGetCatFromPool(CatsProjectile cat)
        {
            cat.gameObject.SetActive(true);
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(1f, _maxDistanceFromPlayer);
            cat.Transform.position = _transform.position + (Vector3)randomDirection;
            cat.ClearHitEnemies();
            _activeCats.Add(cat);
        }

        private void OnReleaseCatToPool(CatsProjectile cat)
        {
            cat.gameObject.SetActive(false);
            _activeCats.Remove(cat);

            if (_activeCats.Count == 0)
            {
                _catsActive = false;

                if (_respawnCoroutine == null)
                {
                    _respawnCoroutine = StartCoroutine(RespawnAfterDelay());
                }
            }
        }

        private void OnDestroyCatObject(CatsProjectile cat)
        {
            Destroy(cat.gameObject);
        }

        protected override void PerformAttack()
        {
            if (!_catsActive && _respawnCoroutine == null)
            {
                SpawnCats();
            }
        }

        private void SpawnCats()
        {
            _catsActive = true;

            for (int i = 0; i < _catsPerSpawn; i++)
            {
                CatsProjectile cat = _catPool.Get();
                cat.Initialize(Data.Damage, this);
                cat.Activate(_catMovementSpeed, _catLifetime, _detectionRadius, transform);
            }
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(Data.Cooldown);
            _respawnCoroutine = null;

            if (!_catsActive)
            {
                SpawnCats();
            }
        }

        public override void LevelUp()
        {
            Level++;

            _catsPerSpawn = _baseCatsPerSpawn + Level / _catsPerLevelDiv;
            _detectionRadius = Mathf.Min(_baseDetectionRadius + (Level * _detectionRadiusPerLevel),
                _maxDetectionRadius);

            switch (Level)
            {
                case 2:
                    Data.Damage *= 1.3f;
                    Data.Cooldown *= 0.85f;
                    break;
            
                case 3:
                    Data.Damage *= 1.6f;
                    Data.Cooldown *= 0.85f;
                    _catMovementSpeed *= 1.2f;
                    break;
            }

            base.LevelUp();
        }

        private void OnDestroy()
        {
            if (_respawnCoroutine != null)
            {
                StopCoroutine(_respawnCoroutine);
                _respawnCoroutine = null;
            }

            foreach (var cat in _activeCats.ToArray())
            {
                if (cat != null)
                {
                    _catPool.Release(cat);
                }
            }

            _activeCats.Clear();
            _targetedEnemies.Clear();
        }

        public bool IsEnemyTargeted(IDamageable enemy)
        {
            return _targetedEnemies.ContainsKey(enemy);
        }

        public void AssignEnemyTarget(IDamageable enemy, CatsProjectile cat)
        {
            if (!_targetedEnemies.ContainsKey(enemy))
            {
                _targetedEnemies.Add(enemy, cat);
            }
            else
            {
                if (_targetedEnemies[enemy] == cat)
                {
                    _targetedEnemies[enemy] = cat;
                }
            }
        }

        public void ReleaseEnemyTarget(IDamageable enemy)
        {
            if (_targetedEnemies.ContainsKey(enemy))
            {
                _targetedEnemies.Remove(enemy);
            }
        }

        public void ClearAllTargets()
        {
            _targetedEnemies.Clear();
        }
    }
}