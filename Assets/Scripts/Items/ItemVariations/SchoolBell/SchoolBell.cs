using System.Collections;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.SchoolBell
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class SchoolBell : Item
    {
        [SerializeField] private SchoolBellProjectile _bellProjectilePrefab;
        [SerializeField] private float _freezeDuration = 3f;
        [SerializeField] private float _ySpawnOffset = 2;
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private float _bellEffectRadius = 5f;

        [SerializeField] private float _effectDurationIncreasePerLevel = 0.2f;
        [SerializeField] private float _radiusIncreasePerLevel = 0.5f;
        [SerializeField] private float _cooldownReductionPerLevel = 0.9f;

        private float _effectDurationMultiplier = 1f;
        private float _radiusMultiplier = 1f;
        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bellProjectilePrefab, _initialPoolSize);
            _transform = transform;

            _effectDurationMultiplier = 1f;
            _radiusMultiplier = 1f;
        }

        protected override void PerformAttack()
        {
            SchoolBellProjectile projectile =
                _projectilePool.GetFromPool<SchoolBellProjectile>(
                    new Vector2(_transform.position.x, _transform.position.y + _ySpawnOffset), Quaternion.identity);

            projectile.SetFreezeDuration(_freezeDuration * _effectDurationMultiplier);
            projectile.SetFreezeRadius(_bellEffectRadius * _radiusMultiplier);
            projectile.SetEnemyLayerMask(_enemyLayerMask);
            projectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(projectile, _freezeDuration * _effectDurationMultiplier));
            projectile.FreezeSurroundingEnemies();
        }

        public override void LevelUp()
        {
            Level++;

            _effectDurationMultiplier += _effectDurationIncreasePerLevel;

            _radiusMultiplier += _radiusIncreasePerLevel;

            Data.Cooldown *= _cooldownReductionPerLevel;

            //base.LevelUp();

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(StatVariations.Radius, _radiusMultiplier);
            ItemStats.SetStatCurrentValue(StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(StatVariations.Duration, _effectDurationMultiplier);

            ItemStats.SetStatNextValue(StatVariations.Radius, _radiusMultiplier + _radiusIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(StatVariations.Duration,
                _effectDurationMultiplier + _effectDurationIncreasePerLevel);
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            WaitForSeconds interval = new WaitForSeconds(lifetime);

            projectile.gameObject.SetActive(true);

            yield return interval;

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }
    }
}