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
        private float _damageMultiplier = 1f;
        private int _projectileDirections = 4;

        private readonly Vector2[] _allDirections = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized, new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
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

            for (int i = 0; i < _projectileDirections; i++)
            {
                Vector2 direction = _allDirections[i];

                GumProjectile projectile =
                    _projectilePool.GetFromPool<GumProjectile>(_transform.position, _transform.rotation);

                if (projectile != null)
                {
                    projectile.SetDirection(direction);
                    projectile.SetSpeed(_projectileSpeed);
                    projectile.Initialize(_damage * _damageMultiplier, this);
                    projectile.Hit += OnProjectileHit;
                }
            }
        }

        private void OnProjectileHit(GumProjectile projectile)
        {
            projectile.Hit -= OnProjectileHit;
            _projectilePool.ReturnToPool(projectile);
        }

        public override void LevelUp()
        {
            _level++;

            switch (_level)
            {
                case 2:
                    _damageMultiplier = 1.25f;
                    _projectileSpeed *= 1.2f;
                    break;

                case 3:
                    _damageMultiplier = 1.5f;
                    Data.Cooldown *= 0.85f;
                    _projectileDirections = 6;
                    break;
            }
        }
    }
}