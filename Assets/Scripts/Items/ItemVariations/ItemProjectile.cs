using System;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemProjectile : MonoBehaviour
    {
        private float _damage;
        private Item _owner;
        private HashSet<IDamageable> _hitEnemies = new();

        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(float damage, Item owner)
        {
            _damage = damage;
            _owner = owner;
        }

        public void ClearHitEnemies()
        {
            _hitEnemies.Clear();
        }

        public void UpdatePosition(Vector2 position)
        {
            transform.position = position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && _hitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(_damage);
            }
        }
    }
}