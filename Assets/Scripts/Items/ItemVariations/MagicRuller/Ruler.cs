using System.Diagnostics;
using Items.BaseClass;
using UnityEngine;
using Items.Pools;
using Debug = UnityEngine.Debug;
using Items.Enums;

namespace Items.ItemVariations.MagicRuller
{
    public class Ruler : Item
    {
        [SerializeField] private RulerProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private int _initialPoolSize;

        private Transform _transform;
        private ItemProjectilePool _projectilePool;
        private float _damage;
        private float _damageMultiplier = 1f;
        private int _projectileCount = 1;
        private float _projectileSpread = 0f;

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
            Vector2 baseDirection = new Vector2(x, 0);

            float[] spreadAngles = new float[_projectileCount];
            if (_projectileCount > 1 && _projectileSpread > 0)
            {
                float totalSpread = _projectileSpread;
                float angleStep = totalSpread / (_projectileCount - 1);
                float startAngle = -totalSpread / 2;

                for (int i = 0; i < _projectileCount; i++)
                {
                    spreadAngles[i] = startAngle + (angleStep * i);
                }
            }
            else
            {
                spreadAngles[0] = 0;
            }

            for (int i = 0; i < _projectileCount; i++)
            {
                Vector2 direction = RotateVector(baseDirection, spreadAngles[i]);

                RulerProjectile projectile =
                    _projectilePool.GetFromPool<RulerProjectile>(_transform.position, _transform.rotation);
                if (projectile != null)
                {
                    projectile.SetDirection(direction);
                    projectile.SetSpeed(_projectileSpeed);
                    projectile.Initialize(_damage * _damageMultiplier, this);
                    projectile.Hit += OnProjectileHit;
                }
            }
        }

        private Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }

        private void OnProjectileHit(RulerProjectile projectile)
        {
            projectile.Hit -= OnProjectileHit;
            _projectilePool.ReturnToPool(projectile);
        }

        public override void LevelUp()
        {
            Level++;

            switch (Level)
            {
                case 2:
                    _damageMultiplier = 1.25f;
                    _projectileSpeed *= 1.2f;
                    break;

                case 3:
                    _damageMultiplier = 1.5f;
                    Data.Cooldown *= 0.85f;
                    _projectileCount = 2;
                    _projectileSpread = 15f;
                    break;
            }

            base.LevelUp();

            if (Level + 1 == 3)
                ItemStats.SetStatStep(StatVariations.ProjectilesCount, 1);
            else
                ItemStats.SetStatStep(StatVariations.ProjectilesCount, 0);
        }
    }
}