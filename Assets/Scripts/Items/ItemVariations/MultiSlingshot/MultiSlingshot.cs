using Items.BaseClass;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations.MultiSlingshot
{
    public class MultiSlingshot : Item
    {
        [SerializeField] private MultiSlingshotItemProjectile _projectilePrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _projectileLifetime = 3f;
        [SerializeField] private float _projectileSpeed = 8f;
        [SerializeField] private int _projectilesPerShot = 3;
        [SerializeField] private float _spreadAngle = 30f;

        [Header("Level Up Settings")] [SerializeField]
        private int _baseProjectilesPerShot = 3;

        [SerializeField] private int _projectilesPerLevelDiv = 2;
        [SerializeField] private float _baseSpreadAngle = 30f;
        [SerializeField] private float _spreadAnglePerLevel = 5f;
        [SerializeField] private float _maxSpreadAngle = 60f;

        [Header("Movement Thresholds")] [SerializeField]
        private float _movementThreshold = 0.1f;

        [Header("Pool Settings")] [SerializeField]
        private int _poolDefaultCapacity = 10;

        [SerializeField] private int _poolMaxSize = 100;

        private ObjectPool<MultiSlingshotItemProjectile> _projectilePool;
        private int _level = 1;

        private void Awake()
        {
            _projectilePool = new ObjectPool<MultiSlingshotItemProjectile>(
                createFunc: CreateProjectile,
                actionOnGet: OnGetProjectileFromPool,
                actionOnRelease: OnReleaseProjectileToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: false,
                defaultCapacity: _poolDefaultCapacity,
                maxSize: _poolMaxSize
            );
        }

        private MultiSlingshotItemProjectile CreateProjectile()
        {
            MultiSlingshotItemProjectile projectile =
                Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);
            projectile.SetPool(_projectilePool);
            return projectile;
        }

        private void OnGetProjectileFromPool(MultiSlingshotItemProjectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.transform.position = transform.position;
            projectile.ClearHitEnemies();
        }

        private void OnReleaseProjectileToPool(MultiSlingshotItemProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(MultiSlingshotItemProjectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        protected override void PerformAttack()
        {
            Vector2 baseDirection = GetAttackDirection();

            float angleStep = _spreadAngle / (_projectilesPerShot - 1);
            float startAngle = -_spreadAngle / 2;

            for (int i = 0; i < _projectilesPerShot; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector2 direction = RotateVector(baseDirection, currentAngle);

                MultiSlingshotItemProjectile projectile = _projectilePool.Get();
                projectile.Initialize(Data.Damage, this);
                projectile.Launch(direction, _projectileSpeed, _projectileLifetime);
            }
        }

        private Vector2 GetAttackDirection()
        {
            if (MovementHandler.IsMoving())
            {
                Vector2 moveDir = new Vector2(
                    MovementHandler.IsMovingLeft() ? -1 : 1,
                    0
                );

                Vector2 inputDirection = GetPlayerInputDirection();
                if (inputDirection.sqrMagnitude > _movementThreshold)
                {
                    return inputDirection.normalized;
                }

                return moveDir;
            }

            return default;
        }

        private Vector2 GetPlayerInputDirection()
        {
            return MovementHandler.GetMoveDirection();
        }

        private Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector2(
                cos * vector.x - sin * vector.y,
                sin * vector.x + cos * vector.y
            );
        }

        protected override void LevelUp()
        {
            _level++;

            _projectilesPerShot = _baseProjectilesPerShot + _level / _projectilesPerLevelDiv;

            _spreadAngle = Mathf.Min(_baseSpreadAngle + (_level * _spreadAnglePerLevel), _maxSpreadAngle);
        }
    }
}