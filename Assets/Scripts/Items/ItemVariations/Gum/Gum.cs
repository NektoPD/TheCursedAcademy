using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Gum
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Gum : Item
    {
        [SerializeField] private GumProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 12f;
        [SerializeField] private int _initialPoolSize;

        private Transform _transform;
        private ItemProjectilePool _projectilePool;
        private float _damage;
        private float _damageMultiplier = 1f;
        private int _projectileDirections = 4;

        private readonly Vector2[] _allDirections = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized, new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        private float _damageIncreasePerLevel = 1.25f;
        private float _projectileSpeedIncreasePerLevel = 1.2f;
        private float _cooldownReductionPerLevel = 0.85f;
        private int _projectileDirectionsPerLevel = 1;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;

        }

        private void Start()
        {
            _damage = Data.Damage;
        }

        protected override void PerformAttack()
        {
            if (MovementHandler == null)
                return;

            for (int i = 0; i < _projectileDirections; i++)
            {
                Vector2 direction = _allDirections[i];

                GumProjectile projectile =
                    _projectilePool.GetFromPool<GumProjectile>(_transform.position, _transform.rotation);

                if (projectile != null)
                {
                    projectile.SetDirection(direction);
                    projectile.SetSpeed(_projectileSpeed);
                    projectile.Initialize(_damage * _damageMultiplier, this);
                    projectile.Hit += OnProjectileHit;
                }
            }
        }

        private void OnProjectileHit(GumProjectile projectile)
        {
            projectile.Hit -= OnProjectileHit;
            _projectilePool.ReturnToPool(projectile);
        }

        public override void LevelUp()
        {
            Level++;

            _damageMultiplier *= _damageIncreasePerLevel;
            _projectileSpeed *= _projectileSpeedIncreasePerLevel;
            Data.Cooldown *= _cooldownReductionPerLevel;
            _projectileDirections += _projectileDirectionsPerLevel;

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, _damageMultiplier);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackDirectionsCount, _projectileDirections);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage, _damageMultiplier * _damageIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _projectileSpeedIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackDirectionsCount,
                _projectileDirections + _projectileDirectionsPerLevel);
        }
    }
}