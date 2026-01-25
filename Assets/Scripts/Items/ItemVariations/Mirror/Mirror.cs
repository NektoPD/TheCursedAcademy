using System.Collections;
using System.Linq;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;
using HealthSystem;
using Items.Enums;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Mirror : Item
    {
        [SerializeField] private MirrorProjectile _mirrorProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private float _detectionRadius = 10f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private bool _enableTargeting = true;
        [SerializeField] private float _baseDetectionRadius = 10f;
        private float _currentDetectionRadius;
        
        private float _damageMultiplier = 1f;
        private ItemProjectilePool _projectilePool;
        private int _projectileCount = 1;
        private float _damageIncreasePerLevel = 1.25f;
        private float _projectileSpeedIncreasePerLevel = 1.2f;
        private float _detectionRadiusIncreasePerLevel = 1.15f;
        private float _cooldownReductionPerLevel = 0.85f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_mirrorProjectilePrefab, _initialPoolSize);
            _currentDetectionRadius = _baseDetectionRadius;
        }

        protected override void PerformAttack()
        {
            Transform[] targets = FindNearestEnemies(_projectileCount);
            if (targets.Length == 0) return;

            for (int i = 0; i < _projectileCount; i++)
            {
                if (i >= targets.Length) break;

                Transform target = targets[i];

                MirrorProjectile mirrorProjectile =
                    _projectilePool.GetFromPool<MirrorProjectile>(transform.position, Quaternion.identity);
                if (mirrorProjectile == null) continue;

                mirrorProjectile.transform.position = transform.position;

                Vector2 direction = (target.position - transform.position).normalized;

                mirrorProjectile.Initialize(RuntimeDamage, this);
                mirrorProjectile.SetDirection(direction);
                mirrorProjectile.SetSpeed(_projectileSpeed);

                if (_enableTargeting)
                {
                    mirrorProjectile.SetTarget(target);
                }

                mirrorProjectile.ClearHitEnemies();

                StartCoroutine(EnableProjectile(mirrorProjectile, _projectileLifetime));
            }
        }

        public override void LevelUp()
        {
            if (Level >= Data.MaxLevel)
            {
                RaiseMaxLevelReached();
                return;
            }
            
            Level++;

            Mods.Multiply(Enums.StatVariations.Damage, _damageIncreasePerLevel);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);
            Mods.Multiply(Enums.StatVariations.Radius, _detectionRadiusIncreasePerLevel);

            _projectileSpeed *= _projectileSpeedIncreasePerLevel;

            RuntimeDamage   = Data.Damage * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);
            _currentDetectionRadius = _baseDetectionRadius * Mods.GetMult(Enums.StatVariations.Radius);

            UpdateStatsValues();
        }


        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Radius, _currentDetectionRadius);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                Data.Damage * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _projectileSpeedIncreasePerLevel);

            ItemStats.SetStatNextValue(Enums.StatVariations.Radius,
                _baseDetectionRadius * (Mods.GetMult(Enums.StatVariations.Radius) * _detectionRadiusIncreasePerLevel));
        }


        private Transform[] FindNearestEnemies(int count)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _currentDetectionRadius, _enemyLayer);

            return colliders
                .Where(c => c.TryGetComponent(out IDamageable _))
                .OrderBy(c => Vector2.Distance(transform.position, c.transform.position))
                .Select(c => c.transform)
                .Take(count)
                .ToArray();
        }

        private IEnumerator EnableProjectile(MirrorProjectile projectile, float lifetime)
        {
            float timer = 0f;

            projectile.gameObject.SetActive(true);
            projectile.Hit += DisableProjectile;

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        private void DisableProjectile(MirrorProjectile projectile)
        {
            projectile.Hit -= _projectilePool.ReturnToPool;
            _projectilePool.ReturnToPool(projectile);
        }
    }
}