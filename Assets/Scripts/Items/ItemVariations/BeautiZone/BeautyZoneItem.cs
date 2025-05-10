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
        [SerializeField] private int _initialPoolSize = 2;
        [SerializeField] private float _spawnYOffset = 0.7f;
        [SerializeField] private float _damageMultiplier = 1f;
        [SerializeField] private float _radiusMultiplier = 1f;
        [SerializeField] private float _durationMultiplier = 1f;
        [SerializeField] private float _projectileReturnDelay = 0.5f;

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
            Vector3 spawnPosition = new Vector3(_transform.position.x, _transform.position.y - _spawnYOffset,
                _transform.position.z);

            BeautyZoneItemProjectile zoneProjectile =
                _projectilePool.GetFromPool<BeautyZoneItemProjectile>(spawnPosition, Quaternion.identity);

            zoneProjectile.Initialize(Data.Damage * _damageMultiplier, this);
            zoneProjectile.ClearHitEnemies();
            zoneProjectile.SetRadius(_zoneRadius * _radiusMultiplier);
            zoneProjectile.SetDuration(_zoneDuration * _durationMultiplier);
            zoneProjectile.Activate();

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

            switch (Level)
            {
                case 2:
                    _damageMultiplier = 1.3f;
                    _radiusMultiplier = 1.2f;
                    _durationMultiplier = 1.2f;
                    Data.Cooldown *= 0.9f;
                    break;

                case 3:
                    _damageMultiplier = 1.6f;
                    _radiusMultiplier = 1.4f;
                    _durationMultiplier = 1.4f;
                    Data.Cooldown *= 0.9f;
                    break;
            }
        }
    }
}