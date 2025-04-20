using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations
{
    public class ChalkProjectile : ItemProjectile
    {
        private Vector2 _targetDirection;
        private float _speed;
        private IObjectPool<ChalkProjectile> _pool;
        private Coroutine _lifetimeCoroutine;
        private HashSet<IDamageable> HitEnemies = new HashSet<IDamageable>();
        
        public void SetPool(IObjectPool<ChalkProjectile> pool)
        {
            _pool = pool;
        }
        
        public void Initialize(float damage, Item owner)
        {
            Damage = damage;
            Owner = owner;
        }
        
        public void ClearHitEnemies()
        {
            HitEnemies.Clear();
        }
        
        public void Launch(Vector2 targetPosition, float speed, float lifetime)
        {
            _speed = speed;
            
            _targetDirection = (targetPosition - (Vector2)transform.position).normalized;
            
            float angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
            }
            
            _lifetimeCoroutine = StartCoroutine(ReturnToPoolAfterTime(lifetime));
        }
        
        private void Update()
        {
            transform.position += (Vector3)_targetDirection * _speed * Time.deltaTime;
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable) &&
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