using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using HealthSystem;
using UnityEngine;

namespace Items.ItemVariations.LaRobba
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class LaRobba : Item
    {
        [SerializeField] private LaRobbaProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 4f;
        [SerializeField] private int _projectilesPerAttack = 1;
        [SerializeField] private int _maxProjectilesPerAttack = 8;
        [SerializeField] private int _initialPoolSize = 16;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private float _detectionRadius = 20f;
        [SerializeField] private float _spawnOffsetAboveScreen = 2f;

        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private float _damageIncreasePerLevel = 1.25f;
        private float _cooldownReductionPerLevel = 0.85f;
        private float _speedIncreasePerLevel = 1.15f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _detectionRadius, _enemyLayer);

            int validCount = 0;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].TryGetComponent(out IDamageable _) ||
                    colliders[i].TryGetComponent(out CharacterLogic.Character _))
                    continue;

                colliders[validCount] = colliders[i];
                validCount++;
            }

            if (validCount == 0) return;

            Vector2 playerPos = _transform.position;
            System.Array.Sort(colliders, 0, validCount, new ClosestToPointComparer(playerPos));

            int toSpawn = Mathf.Min(_projectilesPerAttack, validCount);

            for (int i = 0; i < toSpawn; i++)
            {
                SpawnProjectileAt(colliders[i].transform.position);
            }

            for (int i = toSpawn; i < _projectilesPerAttack; i++)
            {
                int idx = Random.Range(0, validCount);
                SpawnProjectileAt(colliders[idx].transform.position);
            }
        }

        private void SpawnProjectileAt(Vector2 enemyPos)
        {
            Vector2 spawnPos = GetSpawnPositionAboveScreen(enemyPos.x);

            LaRobbaProjectile projectile =
                _projectilePool.GetFromPool<LaRobbaProjectile>(spawnPos, Quaternion.identity);

            if (projectile == null) return;

            projectile.Initialize(RuntimeDamage, this);
            projectile.ClearHitEnemies();
            projectile.Launch(enemyPos, _projectileSpeed);
            projectile.Finished += OnProjectileFinished;
        }

        private Vector2 GetSpawnPositionAboveScreen(float x)
        {
            Camera camera = Camera.main;
            if (camera == null) return new Vector2(x, 10f);
            float topEdge = camera.ViewportToWorldPoint(Vector3.one).y;
            return new Vector2(x, topEdge + _spawnOffsetAboveScreen);
        }

        private void OnProjectileFinished(LaRobbaProjectile projectile)
        {
            projectile.Finished -= OnProjectileFinished;
            _projectilePool.ReturnToPool(projectile);
        }

        public override void LevelUp()
        {
            base.LevelUp();

            if (Level > Data.MaxLevel)
            {
                RaiseMaxLevelReached();
                return;
            }

            Level++;

            Mods.Multiply(Enums.StatVariations.Damage, _damageIncreasePerLevel);
            Mods.Multiply(Enums.StatVariations.AttackSpeed, _cooldownReductionPerLevel);

            _projectileSpeed *= _speedIncreasePerLevel;
            _projectilesPerAttack = Mathf.Min(_projectilesPerAttack + 1, _maxProjectilesPerAttack);

            RuntimeDamage = Data.Damage * Mods.GetMult(Enums.StatVariations.Damage);
            RuntimeCooldown = Data.Cooldown * Mods.GetMult(Enums.StatVariations.AttackSpeed);

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(Enums.StatVariations.Damage, RuntimeDamage);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.AttackSpeed, RuntimeCooldown);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesSpeed, _projectileSpeed);
            ItemStats.SetStatCurrentValue(Enums.StatVariations.ProjectilesCount, _projectilesPerAttack);

            ItemStats.SetStatNextValue(Enums.StatVariations.Damage,
                Data.Damage * (Mods.GetMult(Enums.StatVariations.Damage) * _damageIncreasePerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.AttackSpeed,
                Data.Cooldown * (Mods.GetMult(Enums.StatVariations.AttackSpeed) * _cooldownReductionPerLevel));
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesSpeed,
                _projectileSpeed * _speedIncreasePerLevel);
            ItemStats.SetStatNextValue(Enums.StatVariations.ProjectilesCount,
                Mathf.Min(_projectilesPerAttack + 1, _maxProjectilesPerAttack));
        }

        private struct ClosestToPointComparer : System.Collections.Generic.IComparer<Collider2D>
        {
            private Vector2 _point;

            public ClosestToPointComparer(Vector2 point)
            {
                _point = point;
            }

            public int Compare(Collider2D a, Collider2D b)
            {
                float distA = ((Vector2)a.transform.position - _point).sqrMagnitude;
                float distB = ((Vector2)b.transform.position - _point).sqrMagnitude;
                return distA.CompareTo(distB);
            }
        }
    }
}
