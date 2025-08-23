using System.Collections;
using System.Collections.Generic;
using CharacterLogic;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.BeautiZone
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class BeautyZoneItem : Item
    {
        [SerializeField] private BeautyZoneItemProjectile _beautyZoneProjectilePrefab;
        [SerializeField] private float _zoneRadius = 1.5f;
        [SerializeField] private float _zoneDuration = 2f;
        [SerializeField] private int _initialPoolSize = 2;
        [SerializeField] private float _spawnYOffset = 0.7f;
        [SerializeField] private float _damageMultiplier = 1f;
        [SerializeField] private float _radiusMultiplier = 1f;
        [SerializeField] private float _durationMultiplier = 1f;
        [SerializeField] private float _projectileReturnDelay = 0.5f;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private float _cooldownReductionPerLevel = 0.85f;
        private float _damageMultiplierPerLevel = 1.25f;
        private float _radiusMultiplierPerLevel = 1.2f;
        private float _durationMultiplierPerLevel = 1.2f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_beautyZoneProjectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            StartCoroutine(ActivateZone());
        }

        private IEnumerator ActivateZone()
        {
            Vector3 spawnPosition = new Vector3(_transform.position.x, _transform.position.y - _spawnYOffset,
                _transform.position.z);

            BeautyZoneItemProjectile zoneProjectile =
                _projectilePool.GetFromPool<BeautyZoneItemProjectile>(spawnPosition, Quaternion.identity);

            zoneProjectile.Initialize(Data.Damage * _damageMultiplier, this);
            zoneProjectile.ClearHitEnemies();
            zoneProjectile.SetRadius(_zoneRadius * _radiusMultiplier);
            zoneProjectile.SetDuration(_zoneDuration * _durationMultiplier);
            zoneProjectile.Activate();

            CharacterSoundController.EnableSoundByType(SoundType.Zone);
            StartCoroutine(EnableProjectile(zoneProjectile, _zoneDuration * _durationMultiplier));

            yield return null;
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            projectile.gameObject.SetActive(true);
           
            yield return new WaitForSeconds(lifetime + _projectileReturnDelay);

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }

        public override void LevelUp()
        {
            Level++;

            _damageMultiplier *= _damageMultiplierPerLevel;
            _radiusMultiplier *= _radiusMultiplierPerLevel;
            _durationMultiplier *= _durationMultiplierPerLevel;
            Data.Cooldown *= _cooldownReductionPerLevel;

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Duration, _durationMultiplier);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Radius, _radiusMultiplier);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, _damageMultiplier);

            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.Duration,
                _durationMultiplier * _durationMultiplierPerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.Radius, _radiusMultiplier * _radiusMultiplierPerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.Damage, _damageMultiplier * _damageMultiplierPerLevel);
        }
    }
}