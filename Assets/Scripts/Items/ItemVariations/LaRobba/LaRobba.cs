using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.LaRobba
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class LaRobba : Item
    {
        [SerializeField] private LaRobbaProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 7f;
        [SerializeField] private float _projectileLifetime = 2.5f;
        [SerializeField] private int _projectilesPerAttack = 3;
        [SerializeField] private int _maxProjectilesPerAttack = 8;
        [SerializeField] private int _initialPoolSize = 16;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private float _damageIncreasePerLevel = 1.25f;
        private float _cooldownReductionPerLevel = 0.85f;
        private float _speedIncreasePerLevel = 1.15f;
        private float _lifetimeIncreasePerLevel = 1.1f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            float angleStep = 360f / _projectilesPerAttack;

            for (int i = 0; i < _projectilesPerAttack; i++)
            {
                float angle = (angleStep * i + Random.Range(-10f, 10f)) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                LaRobbaProjectile projectile =
                    _projectilePool.GetFromPool<LaRobbaProjectile>(_transform.position, Quaternion.identity);

                if (projectile == null)
                    continue;

                projectile.Initialize(RuntimeDamage, this);
                projectile.ClearHitEnemies();
                projectile.Launch(direction, _projectileSpeed, _projectileLifetime);
                projectile.Finished += OnProjectileFinished;
            }
        }

        private void OnProjectileFinished(LaRobbaProjectile projectile)
        {
            projectile.Finished -= OnProjectileFinished;
            _projectilePool.ReturnToPool(projectile);
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

            _projectileSpeed *= _speedIncreasePerLevel;
            _projectileLifetime *= _lifetimeIncreasePerLevel;
            _projectilesPerAttack = Mathf.Min(_projectilesPerAttack + 1, _maxProjectilesPerAttack);

            RuntimeDamage = Data.Damage * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectileLifetime, _projectileLifetime);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _projectilesPerAttack);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                Data.Damage * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _speedIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectileLifetime,
                _projectileLifetime * _lifetimeIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                Mathf.Min(_projectilesPerAttack + 1, _maxProjectilesPerAttack));
        }
    }
}
