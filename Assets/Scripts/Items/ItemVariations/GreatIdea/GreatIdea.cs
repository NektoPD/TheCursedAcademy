using System.Collections;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class GreatIdea : Item
    {
        [SerializeField] private GreatIdeaProjectile _ideaProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private float _detectionRadius = 10f;
        [SerializeField] private LayerMask _enemyLayerMask;

        private int _projectileCount = 1;
        private float _damageMultiplier = 1f;
        private ItemProjectilePool _projectilePool;

        private float _damageIncreasePerLevel = 1.25f;
        private float _detectionRadiusIncreasePerLevel = 1.2f;
        private float _cooldownReductionPerLevel = 0.85f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_ideaProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                transform.position,
                _detectionRadius,
                _enemyLayerMask);

            if (enemiesInRange.Length == 0)
                return;

            for (int i = 0; i < enemiesInRange.Length; i++)
            {
                int randomIndex = Random.Range(0, enemiesInRange.Length);
                (enemiesInRange[i], enemiesInRange[randomIndex]) = (enemiesInRange[randomIndex], enemiesInRange[i]);
            }

            int enemiesToTarget = Mathf.Min(_projectileCount, enemiesInRange.Length);

            for (int i = 0; i < enemiesToTarget; i++)
            {
                if (i >= enemiesInRange.Length)
                    break;

                Transform targetEnemy = enemiesInRange[i].transform;
                LaunchProjectileAtTarget(targetEnemy);
            }
        }

        private void LaunchProjectileAtTarget(Transform target)
        {
            GreatIdeaProjectile projectile =
                _projectilePool.GetFromPool<GreatIdeaProjectile>(transform.position, Quaternion.identity);


            projectile.transform.position = target.position;

            projectile.Initialize(Data.Damage * _damageMultiplier, this);
            projectile.SetTarget(target);
            projectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(projectile, _projectileLifetime));
        }

        public override void LevelUp()
        {
            Level++;

            _damageMultiplier *= _damageIncreasePerLevel;
            _detectionRadius *= _detectionRadiusIncreasePerLevel;
            Data.Cooldown *= _cooldownReductionPerLevel;

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(StatVariations.Damage, _damageMultiplier);
            ItemStats.SetStatCurrentValue(StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(StatVariations.Radius, _detectionRadius);

            ItemStats.SetStatNextValue(StatVariations.Damage, _damageMultiplier * _damageIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(StatVariations.Radius, _detectionRadius * _detectionRadiusIncreasePerLevel);
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
    }
}