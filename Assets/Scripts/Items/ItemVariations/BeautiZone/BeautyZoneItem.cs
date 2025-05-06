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
        [SerializeField] private int _level = 1;
        [SerializeField] private float _damageMultiplierPerLevel = 0.2f;
        [SerializeField] private float _radiusMultiplierPerLevel = 0.1f;
        [SerializeField] private float _durationMultiplierPerLevel = 0.1f;
        [SerializeField] private float _spawnYOffset = 0.7f;

        [Header("Gizmo Visualization")] [SerializeField]
        private Color _gizmoFillColor = new Color(0.5f, 0.1f, 0.7f, 0.3f);

        [SerializeField] private Color _gizmoOutlineColor = new Color(0.8f, 0.2f, 1f, 0.8f);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _gizmoFillColor;

            float currentRadius = _zoneRadius * (1 + _radiusMultiplierPerLevel * (_level - 1));

            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - _spawnYOffset,
                transform.position.z);
            Gizmos.DrawSphere(spawnPosition, currentRadius);

            Gizmos.color = _gizmoOutlineColor;
            Gizmos.DrawWireSphere(spawnPosition, currentRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, spawnPosition);
        }

        private IEnumerator ActivateZone()
        {
            float currentRadius = _zoneRadius;
            float currentDuration = _zoneDuration;
            float currentDamage = Data.Damage;

            Vector3 spawnPosition = new Vector3(_transform.position.x, _transform.position.y - _spawnYOffset,
                _transform.position.z);

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

            yield return new WaitForSeconds(lifetime + _projectileReturnDelay);

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