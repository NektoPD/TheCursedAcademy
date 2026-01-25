using System.Collections;
using HealthSystem;
using Items.BaseClass;
using Items.Enums;
using Items.Stats;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations
{
    public class Chalk : Item
    {
        private const int MaxProjectiles = 2;
        
        [SerializeField] private ChalkProjectile _projectilePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _projectileLifetime = 5f;
        [SerializeField] private float _projectileSpeed = 3f;
        [SerializeField] private int _projectilesPerAttack = 3;

        private ObjectPool<ChalkProjectile> _projectilePool;
        private float _damageMultiplier = 1.25f;
        private float _projectileSpeedMultiplier = 1.15f;
        private float _cooldownMultiplier = 0.85f;
        private int _projectileCountIncreasePerLevel = 1;

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
            if (projectile != null)
                Destroy(projectile.gameObject);
        }

        protected override void PerformAttack()
        {
            StartCoroutine(LaunchProjectiles());
        }

        private IEnumerator LaunchProjectiles()
        {
            int count = Mathf.Min(_projectilesPerAttack, MaxProjectiles);

            for (int i = 0; i < count; i++)
            {
                ChalkProjectile projectile = _projectilePool.Get();
                projectile.Initialize(RuntimeDamage, this);
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
            if (Level >= Data.MaxLevel)
            {
                RaiseMaxLevelReached();
                return;
            }
            
            Level++;

            _projectilesPerAttack = Mathf.Min(_projectilesPerAttack + _projectileCountIncreasePerLevel, MaxProjectiles);

            Mods.Multiply(Enums.StatVariations.Damage, _damageMultiplier);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownMultiplier);

            _projectileSpeed *= _projectileSpeedMultiplier;

            RuntimeDamage   = GetBaseStat(Enums.StatVariations.Damage) * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = GetBaseStat(Enums.StatVariations.AttackSpeed) * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }


        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _projectilesPerAttack);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                GetBaseStat(Enums.StatVariations.Damage) * (Mods.GetMult(Enums.StatVariations.Damage) * _damageMultiplier));

            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                GetBaseStat(Enums.StatVariations.AttackSpeed) * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownMultiplier));

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _projectileSpeedMultiplier);

            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                Mathf.Min(_projectilesPerAttack + _projectileCountIncreasePerLevel, MaxProjectiles));
        }

    }
}