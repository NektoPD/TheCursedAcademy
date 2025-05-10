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

        [SerializeField] private float _baseProjectileSpeed = 8f;

        [Header("Movement Thresholds")] [SerializeField]
        private float _movementThreshold = 0.1f;

        [Header("Pool Settings")] [SerializeField]
        private int _poolDefaultCapacity = 10;

        [SerializeField] private int _poolMaxSize = 100;

        private ObjectPool<MultiSlingshotItemProjectile> _projectilePool;
        private Vector2 _lastAttackDirection = Vector2.right;
        private CharacterLogic.InputHandler.CharacterMovementHandler _characterMovementHandler;
        private float _damageMultiplier = 1f;

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

            _characterMovementHandler = FindObjectOfType<CharacterLogic.InputHandler.CharacterMovementHandler>();
            if (_characterMovementHandler != null)
            {
                SubscribeToMovementEvents();
            }

            _projectileSpeed = _baseProjectileSpeed;
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

        private void SubscribeToMovementEvents()
        {
            _characterMovementHandler.MovingLeft += OnMovingLeft;
            _characterMovementHandler.MovingRight += OnMovingRight;
        }

        private void UnsubscribeFromMovementEvents()
        {
            if (_characterMovementHandler != null)
            {
                _characterMovementHandler.MovingLeft -= OnMovingLeft;
                _characterMovementHandler.MovingRight -= OnMovingRight;
            }
        }

        private void OnMovingLeft()
        {
            _lastAttackDirection = Vector2.left;
        }

        private void OnMovingRight()
        {
            _lastAttackDirection = Vector2.right;
        }

        protected override void PerformAttack()
        {
            Vector2 baseDirection = GetAttackDirection();

            if (baseDirection.sqrMagnitude > 0.01f)
            {
                _lastAttackDirection = baseDirection;
            }

            float angleStep = _spreadAngle / (_projectilesPerShot - 1);
            float startAngle = -_spreadAngle / 2;

            if (_projectilesPerShot == 1)
            {
                angleStep = 0;
                startAngle = 0;
            }

            for (int i = 0; i < _projectilesPerShot; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Vector2 direction = RotateVector(baseDirection, currentAngle);

                MultiSlingshotItemProjectile projectile = _projectilePool.Get();
                projectile.Initialize(Data.Damage * _damageMultiplier, this);
                projectile.Launch(direction, _projectileSpeed, _projectileLifetime);
            }
        }

        private Vector2 GetAttackDirection()
        {
            Vector2 inputDirection = GetPlayerInputDirection();

            if (inputDirection.sqrMagnitude > _movementThreshold)
            {
                return inputDirection.normalized;
            }

            return _lastAttackDirection;
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

        public override void LevelUp()
        {
            Level++;

            _projectilesPerShot++;
            _damageMultiplier *= 1.25f;
            Data.Cooldown *= 0.85f;
            _projectileSpeed *= 1.2f;
        }

        private void OnDisable()
        {
            UnsubscribeFromMovementEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromMovementEvents();
        }
    }
}