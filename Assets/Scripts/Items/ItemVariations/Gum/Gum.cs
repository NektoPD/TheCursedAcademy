using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Gum
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Gum : Item
    {
        [SerializeField] private GumProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 12f;
        [SerializeField] private int _initialPoolSize;

        private Transform _transform;
        private ItemProjectilePool _projectilePool;
        private int _level = 1;
        private float _damage;

        private readonly Vector2[] _directions = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down
        };

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

            // Shoot in all four directions
            foreach (Vector2 direction in _directions)
            {
                GumProjectile projectile = 
                    _projectilePool.GetFromPool<GumProjectile>(_transform.position, _transform.rotation);
                
                if (projectile != null)
                {
                    projectile.SetDirection(direction);
                    projectile.SetSpeed(_projectileSpeed);
                    projectile.Initialize(_damage, this);
                    projectile.Hit += OnProjectileHit;
                }
            }
        }

        private void OnProjectileHit(GumProjectile projectile)
        {
            projectile.Hit -= OnProjectileHit;
            _projectilePool.ReturnToPool(projectile);
        }

        protected override void LevelUp()
        {
            _level++;
            // Could add additional logic here to enhance the weapon when leveling up
            // For example, increase damage, add more projectiles, or increase speed
        }
    }
}