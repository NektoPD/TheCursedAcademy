using System;
using HealthSystem;
using UnityEngine;

namespace Items.ItemVariations.Gum
{
    public class GumProjectile : ItemProjectile
    {
        private readonly float _deactivationTimer = 3f;
        private Vector2 _direction;
        private float _speed;
        private float _currentTimer;

        public event Action<GumProjectile> Hit;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            _currentTimer = 0;
            ClearHitEnemies();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        private void Update()
        {
            Transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

            _currentTimer += Time.deltaTime;

            if (_currentTimer >= _deactivationTimer)
                Hit?.Invoke(this);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                if (HitEnemies.Add(damageable))
                {
                    damageable.TakeDamage(Damage);
                }
            }
        }
    }
}