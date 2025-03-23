using System.Collections;
using System.Linq;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;
using HealthSystem;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Mirror : Item
    {
        [SerializeField] private MirrorProjectile _mirrorProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private float _detectionRadius = 10f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private bool _enableTargeting = true;

        private int _level = 1;
        private float _damageMultiplier = 1f;
        private ItemProjectilePool _projectilePool;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_mirrorProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            Transform target = FindNearestEnemy();
            if (target == null) return;

            MirrorProjectile mirrorProjectile =
                _projectilePool.GetFromPool<MirrorProjectile>(transform.position, Quaternion.identity);
            if (mirrorProjectile == null) return;

            mirrorProjectile.transform.position = transform.position;

            Vector2 direction = (target.position - transform.position).normalized;

            mirrorProjectile.Initialize(Data.Damage * _damageMultiplier, this);
            mirrorProjectile.SetDirection(direction);
            mirrorProjectile.SetSpeed(_projectileSpeed);

            if (_enableTargeting)
            {
                mirrorProjectile.SetTarget(target);
            }

            mirrorProjectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(mirrorProjectile, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _enemyLayer);

            return colliders
                .Where(c => c.TryGetComponent(out IDamageable _))
                .OrderBy(c => Vector2.Distance(transform.position, c.transform.position))
                .Select(c => c.transform)
                .FirstOrDefault();
        }

        private IEnumerator EnableProjectile(MirrorProjectile projectile, float lifetime)
        {
            float timer = 0f;

            projectile.gameObject.SetActive(true);
            projectile.Hit += DisableProjectile;

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        private void DisableProjectile(MirrorProjectile projectile)
        {
            projectile.Hit -= _projectilePool.ReturnToPool;
            _projectilePool.ReturnToPool(projectile);
        }
    }
}