using System.Collections.Generic;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class FireblastProjectile : MonoBehaviour
    {
        private float _damage;
        private float _lifetime;
        private Rigidbody2D _rb;
        private readonly HashSet<IDamageable> _hitEnemies = new HashSet<IDamageable>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        public void Launch(Vector2 direction, float speed, float damage, float lifetime)
        {
            _damage = damage;
            _lifetime = lifetime;
            _rb.velocity = direction.normalized * speed;
            Destroy(gameObject, _lifetime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                _hitEnemies.Add(damageable))
            {
                damageable.TakeDamage(_damage);
            }
        }
    }
}
