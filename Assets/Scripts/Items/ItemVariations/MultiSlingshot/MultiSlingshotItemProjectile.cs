using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations.MultiSlingshot
{
    public class MultiSlingshotItemProjectile : ItemProjectile
    {
        private Vector2 _direction;
        private float _speed;
        private IObjectPool<MultiSlingshotItemProjectile> _pool;
        private Coroutine _lifetimeCoroutine;
        private new List<IDamageable> HitEnemies = new List<IDamageable>();

        public void SetPool(IObjectPool<MultiSlingshotItemProjectile> pool)
        {
            _pool = pool;
        }

        public override void Initialize(float damage, Item owner)
        {
            Damage = damage;
            Owner = owner;
        }

        public override void ClearHitEnemies()
        {
            HitEnemies.Clear();
        }

        public void Launch(Vector2 direction, float speed, float lifetime)
        {
            _speed = speed;
            _direction = direction.normalized;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
            }

            _lifetimeCoroutine = StartCoroutine(ReturnToPoolAfterTime(lifetime));
        }

        private void Update()
        {
            transform.position += (Vector3)_direction * _speed * Time.deltaTime;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out CharacterLogic.Character character))
            {
                damageable?.TakeDamage(Damage);
            }
        }

        private IEnumerator ReturnToPoolAfterTime(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }
        }
    }
}