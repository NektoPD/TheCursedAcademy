using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations
{
    public class ItemProjectile : MonoBehaviour
    {
        private float _damage;
        private Item _owner;
        private HashSet<IDamageable> _hitEnemies = new();

        public void Initialize(float damage, Item owner)
        {
            _damage = damage;
            _owner = owner;
        }

        public void ClearHitEnemies()
        {
            _hitEnemies.Clear();
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