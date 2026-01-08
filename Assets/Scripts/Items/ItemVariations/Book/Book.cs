using System.Collections;
using System.Collections.Generic;
using CharacterLogic;
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
        
        [SerializeField] private float _baseProjectileSpeed = 5f;
        [SerializeField] private float _baseProjectileLifetime = 5f;
        [SerializeField] private int _baseProjectileCount = 2;

        private float _currentProjectileSpeed;
        private float _currentProjectileLifetime;
        private int _currentProjectileCount;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;
        private float _damageIncreasePerLevel = 1.15f;
        private float _projectileSpeedIncreasePerLevel = 1.1f;
        private float _projectileLifetimeIncreasePerLevel = 1.1f;
        private int _projectileCountIncreasePerLevel = 1;
        private float _cooldownReductionPerLevel = 0.85f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bookProjectilePrefab, _initialPoolSize);
            _transform = transform;
            
            _currentProjectileSpeed = _baseProjectileSpeed;
            _currentProjectileLifetime = _baseProjectileLifetime;
            _currentProjectileCount = _baseProjectileCount;
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
                    projectile.Rigidbody2D.velocity = direction * _currentProjectileSpeed;
                }

                StartCoroutine(EnableProjectile(projectile, _currentProjectileLifetime));

                yield return interval;
            }
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            float timer = 0f;

            Transform originalParent = projectile.transform.parent;

            projectile.transform.SetParent(null);
            projectile.gameObject.SetActive(true);
            CharacterSoundController.EnableSoundByType(SoundType.Book);

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

            Mods.Multiply(Enums.StatVariations.Damage, _damageIncreasePerLevel);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);

            _currentProjectileSpeed *= _projectileSpeedIncreasePerLevel;
            _currentProjectileLifetime *= _projectileLifetimeIncreasePerLevel;

            if (Level <= 2)
                _currentProjectileCount += _projectileCountIncreasePerLevel;

            RuntimeDamage   = GetBaseStat(Enums.StatVariations.Damage) * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = GetBaseStat(Enums.StatVariations.AttackSpeed) * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }


        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _currentProjectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectileLifetime, _currentProjectileLifetime);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _currentProjectileCount);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                GetBaseStat(Enums.StatVariations.Damage) * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                GetBaseStat(Enums.StatVariations.AttackSpeed) * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _currentProjectileSpeed * _projectileSpeedIncreasePerLevel);

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectileLifetime,
                _currentProjectileLifetime * _projectileLifetimeIncreasePerLevel);

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                (Level <= 2) ? _currentProjectileCount + _projectileCountIncreasePerLevel : _currentProjectileCount);
        }

    }
}