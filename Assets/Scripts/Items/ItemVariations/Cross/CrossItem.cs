using System.Linq;
using HealthSystem;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Cross
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class CrossItem : Item
    {
        [SerializeField] private CrossProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 9f;
        [SerializeField] private float _maxTravelDistance = 4f;
        [SerializeField] private float _detectionRadius = 12f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private int _projectilesPerAttack = 1;
        [SerializeField] private int _initialPoolSize = 6;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;
        private float _currentDetectionRadius;

        private float _damageIncreasePerLevel = 1.25f;
        private float _cooldownReductionPerLevel = 0.85f;
        private float _speedIncreasePerLevel = 1.15f;
        private float _radiusIncreasePerLevel = 1.12f;
        private float _rangeIncreasePerLevel = 1.1f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;
            _currentDetectionRadius = _detectionRadius;
        }

        protected override void PerformAttack()
        {
            Transform[] targets = FindNearestEnemies(_projectilesPerAttack);
            if (targets.Length == 0)
            {
                SpawnProjectile((Vector2)_transform.position + Random.insideUnitCircle.normalized);
                return;
            }

            foreach (Transform target in targets)
            {
                Vector2 direction = ((Vector2)target.position - (Vector2)_transform.position).normalized;
                SpawnProjectile(direction);
            }
        }

        private void SpawnProjectile(Vector2 direction)
        {
            CrossProjectile projectile =
                _projectilePool.GetFromPool<CrossProjectile>(_transform.position, Quaternion.identity);

            if (projectile == null)
                return;

            projectile.Transform.SetParent(null);
            projectile.Initialize(RuntimeDamage, this);
            projectile.ClearHitEnemies();
            projectile.Launch(direction, _projectileSpeed, _maxTravelDistance, _transform);
            projectile.Finished += OnProjectileFinished;
        }

        private void OnProjectileFinished(CrossProjectile projectile)
        {
            projectile.Finished -= OnProjectileFinished;
            projectile.Transform.SetParent(_projectilePool.transform);
            _projectilePool.ReturnToPool(projectile);
        }

        private Transform[] FindNearestEnemies(int count)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _currentDetectionRadius, _enemyLayer);

            return colliders
                .Where(c => c.TryGetComponent(out IDamageable _))
                .OrderBy(c => Vector2.Distance(_transform.position, c.transform.position))
                .Select(c => c.transform)
                .Take(count)
                .ToArray();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            if (Level > Data.MaxLevel)
            {
                RaiseMaxLevelReached();
                return;
            }

            Level++;

            Mods.Multiply(Enums.StatVariations.Damage, _damageIncreasePerLevel);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);
            Mods.Multiply(Enums.StatVariations.Radius, _radiusIncreasePerLevel);

            _projectileSpeed *= _speedIncreasePerLevel;
            _maxTravelDistance *= _rangeIncreasePerLevel;
            _currentDetectionRadius = _detectionRadius * Mods.GetMult(Enums.StatVariations.Radius);

            if (Level % 2 == 0)
                _projectilesPerAttack = Mathf.Min(_projectilesPerAttack + 1, 3);

            RuntimeDamage = Data.Damage * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Radius, _currentDetectionRadius);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackRange, _maxTravelDistance);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _projectilesPerAttack);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                Data.Damage * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _speedIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.Radius,
                _detectionRadius * (Mods.GetMult(Enums.StatVariations.Radius) * _radiusIncreasePerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackRange,
                _maxTravelDistance * _rangeIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                (Level % 2 == 0) ? Mathf.Min(_projectilesPerAttack + 1, 3) : _projectilesPerAttack);
        }
    }
}
