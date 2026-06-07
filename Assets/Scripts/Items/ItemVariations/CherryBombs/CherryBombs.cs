using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.CherryBombs
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class CherryBombs : Item
    {
        [SerializeField] private CherryBombsProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private float _fuseDuration = 1.5f;
        [SerializeField] private int _projectilesPerAttack = 1;
        [SerializeField] private int _initialPoolSize = 8;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private float _damageIncreasePerLevel = 1.25f;
        private float _cooldownReductionPerLevel = 0.85f;
        private float _speedIncreasePerLevel = 1.1f;
        private float _fuseReductionPerLevel = 0.9f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            Vector2 direction = MovementHandler != null && MovementHandler.IsMoving()
                ? MovementHandler.GetMoveDirection()
                : Vector2.right;

            float spreadAngle = 15f;

            for (int i = 0; i < _projectilesPerAttack; i++)
            {
                float offset = 0f;
                if (_projectilesPerAttack > 1)
                    offset = Mathf.Lerp(-spreadAngle, spreadAngle, (float)i / (_projectilesPerAttack - 1));

                float rad = offset * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(
                    direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad),
                    direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad)
                ).normalized;

                CherryBombsProjectile projectile =
                    _projectilePool.GetFromPool<CherryBombsProjectile>(_transform.position, Quaternion.identity);

                if (projectile == null)
                    continue;

                projectile.Transform.SetParent(null);
                projectile.Initialize(RuntimeDamage, this);
                projectile.ClearHitEnemies();
                projectile.Launch(_transform.position, dir, _projectileSpeed);
                projectile.Finished += OnProjectileFinished;
            }
        }

        private void OnProjectileFinished(CherryBombsProjectile projectile)
        {
            projectile.Finished -= OnProjectileFinished;
            projectile.Transform.SetParent(_projectilePool.transform);
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
            _fuseDuration = Mathf.Max(0.35f, _fuseDuration * _fuseReductionPerLevel);

            if (Level % 2 == 0)
                _projectilesPerAttack = Mathf.Min(_projectilesPerAttack + 1, 4);

            RuntimeDamage = Data.Damage * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Duration, _fuseDuration);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _projectilesPerAttack);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                Data.Damage * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _speedIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.Duration,
                Mathf.Max(0.35f, _fuseDuration * _fuseReductionPerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                (Level % 2 == 0) ? Mathf.Min(_projectilesPerAttack + 1, 4) : _projectilesPerAttack);
        }
    }
}
