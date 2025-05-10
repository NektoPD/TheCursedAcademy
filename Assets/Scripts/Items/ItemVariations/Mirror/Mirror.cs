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

        private float _damageMultiplier = 1f;
        private ItemProjectilePool _projectilePool;
        private int _projectileCount = 1;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_mirrorProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            Transform[] targets = FindNearestEnemies(_projectileCount);
            if (targets.Length == 0) return;

            for (int i = 0; i < _projectileCount; i++)
            {
                if (i >= targets.Length) break;

                Transform target = targets[i];
                
                MirrorProjectile mirrorProjectile =
                    _projectilePool.GetFromPool<MirrorProjectile>(transform.position, Quaternion.identity);
                if (mirrorProjectile == null) continue;

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
        }

        public override void LevelUp()
        {
            Level++;

            switch (Level)
            {
                case 2:
                    _damageMultiplier = 1.25f;
                    _projectileSpeed *= 1.2f;
                    _detectionRadius *= 1.15f;
                    break;

                case 3:
                    _damageMultiplier = 1.5f;
                    Data.Cooldown *= 0.85f;
                    _projectileCount = 2;
                    break;
            }
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

        private Transform[] FindNearestEnemies(int count)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _enemyLayer);

            return colliders
                .Where(c => c.TryGetComponent(out IDamageable _))
                .OrderBy(c => Vector2.Distance(transform.position, c.transform.position))
                .Select(c => c.transform)
                .Take(count)
                .ToArray();
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