using System;
using HealthSystem;
using UnityEngine;

namespace Items.ItemVariations.MagicRuller
{
    public class RulerProjectile : ItemProjectile
    {
        private Vector2 _direction;
        private float _speed;
        private Transform _transform;
        private float _diactivationTimer = 30f;
        private float _currentTimer;

        public event Action<RulerProjectile> Hit;

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }

        private void OnEnable()
        {
            _currentTimer = 0;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        private void Update()
        {
            _transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

            _currentTimer += Time.deltaTime;

            if (_currentTimer >= _diactivationTimer)
                Hit?.Invoke(this);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage);
                Hit?.Invoke(this);
            }
        }
    }
}