using System.Collections;
using System.Collections.Generic;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Book
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Book : Item
    {
        [SerializeField] private BookProjectile _bookProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private float _spawnOffset = 0.5f;
        [SerializeField] private float _projectileLifetime = 5f;
        [SerializeField] private int _projectileCount = 2;
        [SerializeField] private float _spreadAngle = 30f;
        [SerializeField] private int _initialPoolSize = 6;
        [SerializeField] private float _damageMultiplier = 1f;
        [SerializeField] private float _projectileSpawnInterval = 0.1f;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;
        private float _damageIncreasePerLevel = 1.25f;
        private float _projectileSpeedIncreasePerLevel = 1.1f;
        private float _projectileLifetimeIncreasePerLevel = 1.1f;
        private int _projectileCountIncreasePerLevel = 1;
        private float _cooldownReductionPerLevel = 0.85f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bookProjectilePrefab, _initialPoolSize);
            _transform = transform;
            
        }

        protected override void PerformAttack()
        {
            StartCoroutine(SpawnProjectiles());
        }

        private IEnumerator SpawnProjectiles()
        {
            float angleStep = _spreadAngle / (_projectileCount - 1);
            float startAngle = -_spreadAngle / 2;
            WaitForSeconds interval = new WaitForSeconds(_projectileSpawnInterval);

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector3 spawnPosition = _transform.position +
                                        Quaternion.Euler(0, 0, currentAngle) * Vector3.down * _spawnOffset;

                BookProjectile projectile =
                    _projectilePool.GetFromPool<BookProjectile>(spawnPosition, Quaternion.identity);

                projectile.Initialize(Data.Damage * _damageMultiplier, this);
                projectile.ClearHitEnemies();

                Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * Vector2.up;
                if (projectile.Rigidbody2D != null)
                {
                    projectile.Rigidbody2D.velocity = direction * (_projectileSpeed);
                }

                StartCoroutine(EnableProjectile(projectile, _projectileLifetime));

                yield return interval;
            }
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            float timer = 0f;

            Transform originalParent = projectile.transform.parent;

            projectile.transform.SetParent(null);
            projectile.gameObject.SetActive(true);

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (projectile && projectile.gameObject.activeSelf)
            {
                projectile.transform.SetParent(originalParent);
                _projectilePool.ReturnToPool(projectile);
            }
        }

        public override void LevelUp()
        {
            Level++;

            _damageMultiplier *= _damageIncreasePerLevel;
            _projectileSpeed *= _projectileSpeedIncreasePerLevel;
            _projectileLifetime *= _projectileLifetimeIncreasePerLevel;
            _projectileCount += _projectileCountIncreasePerLevel;
            Data.Cooldown *= _cooldownReductionPerLevel;
            
            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(StatVariations.Damage, _damageMultiplier);
            ItemStats.SetStatCurrentValue(StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(StatVariations.ProjectileLifetime, _projectileLifetime);
            ItemStats.SetStatCurrentValue(StatVariations.ProjectilesCount, _projectileCount);

            ItemStats.SetStatNextValue(StatVariations.Damage, _damageMultiplier * _damageIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(StatVariations.ProjectilesSpeed, _projectileSpeed * _projectileSpeedIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.ProjectileLifetime, _projectileLifetime * _projectileLifetimeIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.ProjectilesCount, _projectileCount + _projectileCountIncreasePerLevel);
        }
    }
}