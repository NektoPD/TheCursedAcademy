using System.Collections;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.SchoolBell
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class SchoolBell : Item
    {
        [SerializeField] private SchoolBellProjectile _bellProjectilePrefab;
        [SerializeField] private float _freezeDuration = 3f;

        [SerializeField] private float _ySpawnOffset = 2;
        [SerializeField] private int _initialPoolSize = 5;

        //[SerializeField] private float _detectionRadius = 8f;
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private float _bellEffectRadius = 5f;

        private float _effectDurationMultiplier = 1f;
        private ItemProjectilePool _projectilePool;
        private Transform _transform;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_bellProjectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            SchoolBellProjectile projectile =
                _projectilePool.GetFromPool<SchoolBellProjectile>(
                    new Vector2(_transform.position.x, _transform.position.y + _ySpawnOffset), Quaternion.identity);

            projectile.Initialize(Data.Damage, this);
            projectile.SetFreezeDuration(_freezeDuration * _effectDurationMultiplier);
            projectile.SetFreezeRadius(_bellEffectRadius);
            projectile.SetEnemyLayerMask(_enemyLayerMask);
            projectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(projectile, _freezeDuration * _effectDurationMultiplier));
            projectile.FreezeSurroundingEnemies();
        }

        protected override void LevelUp()
        {
            _effectDurationMultiplier += 0.2f;
            _bellEffectRadius += 0.5f;
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            projectile.gameObject.SetActive(true);

            yield return new WaitForSeconds(lifetime);

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }
    }
}