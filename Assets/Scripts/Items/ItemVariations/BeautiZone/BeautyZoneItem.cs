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
            Vector3 spawnPosition = new Vector3(_transform.position.x, _transform.position.y - _spawnYOffset, _transform.position.z);

            var zoneProjectile = _projectilePool.GetFromPool<BeautyZoneItemProjectile>(spawnPosition, Quaternion.identity);

            float radius   = _zoneRadius   * Mods.GetMult(Enums.StatVariations.Radius);
            float duration = _zoneDuration * Mods.GetMult(Enums.StatVariations.Duration);

            zoneProjectile.Initialize(RuntimeDamage, this);
            zoneProjectile.ClearHitEnemies();
            zoneProjectile.SetRadius(radius);
            zoneProjectile.SetDuration(duration);
            zoneProjectile.Activate();

            CharacterSoundController.EnableSoundByType(SoundType.Zone);
            StartCoroutine(EnableProjectile(zoneProjectile, duration));

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

            Mods.Multiply(Enums.StatVariations.Damage, _damageMultiplierPerLevel);
            Mods.Multiply(Enums.StatVariations.Radius, _radiusMultiplierPerLevel);
            Mods.Multiply(Enums.StatVariations.Duration, _durationMultiplierPerLevel);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);

            RuntimeDamage   = GetBaseStat(Enums.StatVariations.Damage) * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = GetBaseStat(Enums.StatVariations.AttackSpeed) * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            // Current
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Radius, _zoneRadius * Mods.GetMult(Enums.StatVariations.Radius));
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Duration, _zoneDuration * Mods.GetMult(Enums.StatVariations.Duration));

            // Next
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                GetBaseStat(Enums.StatVariations.AttackSpeed) * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                GetBaseStat(Enums.StatVariations.Damage) * (Mods.GetMult(Enums.StatVariations.Damage) * _damageMultiplierPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.Radius,
                _zoneRadius * (Mods.GetMult(Enums.StatVariations.Radius) * _radiusMultiplierPerLevel));

            ItemStats.SetStatNextValue(Enums.StatVariations.Duration,
                _zoneDuration * (Mods.GetMult(Enums.StatVariations.Duration) * _durationMultiplierPerLevel));
        }
    }
}