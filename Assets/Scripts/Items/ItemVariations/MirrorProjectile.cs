using System;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations
{
    public class MirrorProjectile : ItemProjectile
    {
        [SerializeField] private float _reflectionDamageMultiplier = 1.5f;
        [SerializeField] private LayerMask _reflectableLayers;
        [SerializeField] private bool _followTarget = true;

        private Vector2 _direction;
        private float _speed;
        private Transform _transform;
        private Transform _targetTransform;

        public event Action<MirrorProjectile> Hit;

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
        }

        private void Update()
        {
            if (_followTarget && _targetTransform != null && _targetTransform.gameObject.activeSelf)
            {
                _direction = (_targetTransform.position - _transform.position).normalized;
            }

            _transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable.TakeDamage(Damage);
                Hit?.Invoke(this);
            }
        }

        private void OnDisable()
        {
            _targetTransform = null;
        }
    }
}