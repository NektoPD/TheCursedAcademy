using System.Collections;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Book
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Book : Item
    {
        [SerializeField] private BookProjectile _bookProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private float _spawnOffset = 0.5f;
        [SerializeField] private float _projectileLifetime = 5f;
        [SerializeField] private int _projectileCount = 2;
        [SerializeField] private float _spreadAngle = 30f;
        [SerializeField] private int _initialPoolSize = 6;

        [SerializeField] private int _level = 1;
        [SerializeField] private float _damageMultiplierPerLevel = 0.2f;
        [SerializeField] private float _cooldownReductionPerLevel = 0.1f;
        [SerializeField] private float _projectileSpawnInterval = 0.1f;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bookProjectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            StartCoroutine(SpawnProjectiles());
        }

        private IEnumerator SpawnProjectiles()
        {
            float angleStep = _spreadAngle / (_projectileCount - 1);
            float startAngle = -_spreadAngle / 2;
            WaitForSeconds interval = new WaitForSeconds(_projectileSpawnInterval);

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector3 spawnPosition = _transform.position +
                                        Quaternion.Euler(0, 0, currentAngle) * Vector3.down * _spawnOffset;

                BookProjectile projectile =
                    _projectilePool.GetFromPool<BookProjectile>(spawnPosition, Quaternion.identity);
                projectile.Initialize(Data.Damage * _damageMultiplierPerLevel, this);
                projectile.ClearHitEnemies();

                Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * Vector2.up;
                if (projectile.Rigidbody2D != null)
                {
                    projectile.Rigidbody2D.velocity = direction * _projectileSpeed;
                }

                StartCoroutine(EnableProjectile(projectile, _projectileLifetime));

                yield return interval;
            }
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            float timer = 0f;

            projectile.gameObject.SetActive(true);

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
                yield return null;
            }

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