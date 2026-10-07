using System.Collections.Generic;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class AbilityProjectile : MonoBehaviour
    {
        protected Rigidbody2D Rb;
        protected float Damage;
        protected readonly HashSet<IDamageable> HitEnemies = new HashSet<IDamageable>();

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.gravityScale = 0f;
            Rb.freezeRotation = true;
        }

        public virtual void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            Damage = damage;
            Rb.velocity = direction.normalized * speed;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                HitEnemies.Add(damageable))
            {
                ApplyEffect(damageable);
            }
        }

        protected virtual void ApplyEffect(IDamageable target)
        {
            target.TakeDamage(Damage);
        }
    }
}
