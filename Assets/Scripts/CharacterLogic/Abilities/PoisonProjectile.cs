using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PoisonProjectile : MonoBehaviour
    {
        private const int PoisonTicks = 5;
        private const float MaxLifetime = 3f;

        private float _totalDamage;
        private float _poisonDuration;
        private Rigidbody2D _rb;
        private readonly HashSet<IDamageable> _poisoned = new HashSet<IDamageable>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        public void Launch(Vector2 direction, float speed, float totalDamage, float poisonDuration)
        {
            _totalDamage = totalDamage;
            _poisonDuration = poisonDuration;
            _rb.velocity = direction.normalized * speed;
            Destroy(gameObject, MaxLifetime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                _poisoned.Add(damageable))
            {
                StartCoroutine(ApplyPoison(damageable));
            }
        }

        private IEnumerator ApplyPoison(IDamageable target)
        {
            float tickDamage = _totalDamage / PoisonTicks;
            float tickInterval = _poisonDuration / PoisonTicks;

            for (int i = 0; i < PoisonTicks; i++)
            {
                if ((target as Object) == null) yield break;
                target.TakeDamage(tickDamage);
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }
}
