using System.Diagnostics;
using Items.BaseClass;
using UnityEngine;
using Items.Pools;
using Debug = UnityEngine.Debug;

namespace Items.ItemVariations.MagicRuller
{
    public class Ruler : Item
    {
        [SerializeField] private RulerProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private int _initialPoolSize;

        private Transform _transform;
        private ItemProjectilePool _projectilePool;
        private int _level = 1;
        private float _damage;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        private void Start()
        {
            _damage = Data.Damage;
        }

        protected override void PerformAttack()
        {
            if (MovementHandler == null)
                return;

            float x = MovementHandler.IsMovingLeft() ? -1f : 1f;

            Vector2 direction = new Vector2(x, 0);

            RulerProjectile projectile =
                _projectilePool.GetFromPool<RulerProjectile>(_transform.position, _transform.rotation);
            if (projectile != null)
            {
                projectile.SetDirection(direction);
                projectile.SetSpeed(_projectileSpeed);
                projectile.Hit += OnProjectileHit;
            }
        }

        private void OnProjectileHit(RulerProjectile projectile)
        {
            projectile.Hit -= OnProjectileHit;
            _projectilePool.ReturnToPool(projectile);
        }

        protected override void LevelUp()
        {
            _level++;
        }
    }
}