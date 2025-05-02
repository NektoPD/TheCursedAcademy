using System;
using UnityEngine;
using HealthSystem;
using Items.BaseClass;

namespace Items.ItemVariations
{
    public class GreatIdeaProjectile : ItemProjectile
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _hitAnimationTrigger = "Hit";

        private Transform _target;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_target)
                Transform.position = _target.position;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage);

                if (_animator != null)
                {
                    _animator.SetTrigger(_hitAnimationTrigger);
                }
            }
        }

        private void OnDisable()
        {
            _target = null;
        }
    }
}