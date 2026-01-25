using System.Collections;
using CharacterLogic;
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
        
        private float _currentEffectDurationMult = 1f;
        private float _currentRadiusMult = 1f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bellProjectilePrefab, _initialPoolSize);
            _transform = transform;

            _effectDurationMultiplier = 1f;
            _radiusMultiplier = 1f;
            
            _currentEffectDurationMult = 1f;
            _currentRadiusMult = 1f;
        }

        protected override void PerformAttack()
        {
            SchoolBellProjectile projectile =
                _projectilePool.GetFromPool<SchoolBellProjectile>(
                    new Vector2(_transform.position.x, _transform.position.y + _ySpawnOffset), Quaternion.identity);

            projectile.SetFreezeDuration(_freezeDuration * _currentEffectDurationMult);
            projectile.SetFreezeRadius(_bellEffectRadius * _currentRadiusMult);
            StartCoroutine(EnableProjectile(projectile, _freezeDuration * _currentEffectDurationMult));

            projectile.SetEnemyLayerMask(_enemyLayerMask);
            projectile.ClearHitEnemies();

            projectile.FreezeSurroundingEnemies();
        }

        public override void LevelUp()
        {
            if (Level > Data.MaxLevel)
            {
                RaiseMaxLevelReached();
                return;
            }
            
            Level++;

            _currentEffectDurationMult += _effectDurationIncreasePerLevel;
            _currentRadiusMult += _radiusIncreasePerLevel;

            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Radius, _currentRadiusMult);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Duration, _currentEffectDurationMult);

            ItemStats.SetStatNextValue(Enums.StatVariations.Radius, _currentRadiusMult + _radiusIncreasePerLevel);

            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.Duration,
                _currentEffectDurationMult + _effectDurationIncreasePerLevel);
        }


        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            WaitForSeconds interval = new WaitForSeconds(lifetime);

            projectile.gameObject.SetActive(true);
            CharacterSoundController.EnableSoundByType(SoundType.Bell);

            yield return interval;

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }
    }
}