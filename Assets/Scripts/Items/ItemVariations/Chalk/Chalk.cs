using System.Collections;
using HealthSystem;
using Items.BaseClass;
using Items.Stats;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations
{
    public class Chalk : Item
    {
        [SerializeField] private ChalkProjectile _projectilePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _projectileLifetime = 5f;
        [SerializeField] private float _projectileSpeed = 3f;
        [SerializeField] private int _projectilesPerAttack = 3;

        private ObjectPool<ChalkProjectile> _projectilePool;

        private void Awake()
        {
            _projectilePool = new ObjectPool<ChalkProjectile>(
                createFunc: CreateProjectile,
                actionOnGet: OnGetProjectileFromPool,
                actionOnRelease: OnReleaseProjectileToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        private ChalkProjectile CreateProjectile()
        {
            ChalkProjectile projectile = Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);
            projectile.SetPool(_projectilePool);
            return projectile;
        }

        private void OnGetProjectileFromPool(ChalkProjectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.transform.position = transform.position;
            projectile.ClearHitEnemies();
        }

        private void OnReleaseProjectileToPool(ChalkProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(ChalkProjectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        protected override void PerformAttack()
        {
            StartCoroutine(LaunchProjectiles());
        }

        private IEnumerator LaunchProjectiles()
        {
            for (int i = 0; i < _projectilesPerAttack; i++)
            {
                ChalkProjectile projectile = _projectilePool.Get();
                projectile.Initialize(Data.Damage, this);
                projectile.Launch(FindNearestTarget(), _projectileSpeed, _projectileLifetime);

                yield return new WaitForSeconds(0.2f);
            }
        }

        private Vector2 FindNearestTarget()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20f);

            IDamageable nearestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IDamageable damageable) &&
                    !collider.TryGetComponent(out CharacterLogic.Character character))
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nearestTarget = damageable;
                    }
                }
            }

            if (nearestTarget != null && nearestTarget is MonoBehaviour targetBehaviour)
            {
                return targetBehaviour.transform.position;
            }

            return (Vector2)transform.position + Random.insideUnitCircle.normalized * 10f;
        }

        public override void LevelUp()
        {
            Level++;

            _projectilesPerAttack++;

            switch (Level)
            {
                case 2:
                    Data.Damage *= 1.25f;
                    _projectileSpeed *= 1.15f;
                    break;

                case 3:
                    Data.Damage *= 1.25f;
                    Data.Cooldown *= 0.85f;
                    _projectileLifetime *= 1.3f;
                    break;
            }

            base.LevelUp();
        }
    }
}