using System.Collections;
using Items.BaseClass;
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
       // [SerializeField] private float _spawnOffset = 0.5f;
        [SerializeField] private int _initialPoolSize = 2;

        [SerializeField] private int _level = 1;
        [SerializeField] private float _damageMultiplierPerLevel = 0.2f;
        [SerializeField] private float _radiusMultiplierPerLevel = 0.1f;
        [SerializeField] private float _durationMultiplierPerLevel = 0.1f;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

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
            float currentRadius = _zoneRadius * (1 + _radiusMultiplierPerLevel * (_level - 1));
            float currentDuration = _zoneDuration * (1 + _durationMultiplierPerLevel * (_level - 1));
            float currentDamage = Data.Damage * (1 + _damageMultiplierPerLevel * (_level - 1));

            Vector3 spawnPosition = _transform.position;

            BeautyZoneItemProjectile zoneProjectile = 
                _projectilePool.GetFromPool<BeautyZoneItemProjectile>(spawnPosition, Quaternion.identity);

            zoneProjectile.Initialize(currentDamage, this);
            zoneProjectile.ClearHitEnemies();
            zoneProjectile.SetRadius(currentRadius);
            zoneProjectile.SetDuration(currentDuration);
            zoneProjectile.Activate();

            StartCoroutine(EnableProjectile(zoneProjectile, currentDuration));

            yield return null;
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            projectile.gameObject.SetActive(true);

            yield return new WaitForSeconds(lifetime + 0.5f);

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }

        protected override void LevelUp()
        {
            _level++;
        }
    }
}