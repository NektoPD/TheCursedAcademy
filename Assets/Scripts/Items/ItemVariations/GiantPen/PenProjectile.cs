using UnityEngine;
using HealthSystem;
using Items.BaseClass;

namespace Items.ItemVariations
{
    public class PenProjectile : ItemProjectile
    {
        //[SerializeField] private float _damageMultiplier = 1f;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage);
            }
        }
    }
}