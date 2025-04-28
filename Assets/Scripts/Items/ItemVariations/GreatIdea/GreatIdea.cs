using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class GreatIdea : Item
    {
        [SerializeField] private GreatIdeaProjectile _ideaProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private float _detectionRadius = 10f;
        [SerializeField] private LayerMask _enemyLayerMask;

        private int _level = 1;
        private int _projectileCount = 1;
        private float _damageMultiplier = 1f;
        private ItemProjectilePool _projectilePool;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_ideaProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            // Find random enemies within detection radius
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                transform.position,
                _detectionRadius,
                _enemyLayerMask);

            // If no enemies found, return
            if (enemiesInRange.Length == 0)
                return;

            // Shuffle the array to get random enemies
            for (int i = 0; i < enemiesInRange.Length; i++)
            {
                int randomIndex = Random.Range(0, enemiesInRange.Length);
                (enemiesInRange[i], enemiesInRange[randomIndex]) = (enemiesInRange[randomIndex], enemiesInRange[i]);
            }

            // Limit to available projectile count
            int enemiesToTarget = Mathf.Min(_projectileCount, enemiesInRange.Length);

            for (int i = 0; i < enemiesToTarget; i++)
            {
                // Skip if we've run out of valid enemies
                if (i >= enemiesInRange.Length)
                    break;

                Transform targetEnemy = enemiesInRange[i].transform;
                LaunchProjectileAtTarget(targetEnemy);
            }
        }

        private void LaunchProjectileAtTarget(Transform target)
        {
            // Get a projectile from the pool
            GreatIdeaProjectile projectile =
                _projectilePool.GetFromPool<GreatIdeaProjectile>(transform.position, Quaternion.identity);

            // Calculate direction to target
            Vector2 direction = (target.position - transform.position).normalized;

            // Set projectile position to spawn at character
            projectile.transform.position = transform.position;

            // Rotate projectile to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Initialize projectile with damage and direction
            projectile.Initialize(Data.Damage * _damageMultiplier, this);
            projectile.SetDirection(direction, _projectileSpeed);
            projectile.ClearHitEnemies();

            // Activate and set lifetime
            StartCoroutine(EnableProjectile(projectile, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;

            // Increase the number of projectiles every level
            _projectileCount++;

            _damageMultiplier += 0.1f;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}