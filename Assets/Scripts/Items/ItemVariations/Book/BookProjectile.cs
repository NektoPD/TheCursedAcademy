using System.Collections;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.Book
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BookProjectile : ItemProjectile
    {
        [SerializeField] private float _rotationSpeed = 360f;
       // [SerializeField] private ParticleSystem _hitEffect;
        
        private Rigidbody2D _rb;
        private bool _isInitialized = false;

        public Rigidbody2D Rigidbody2D => _rb;
        
        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            
            Collider2D collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
            
            _rb.gravityScale = 0.5f;
            _rb.drag = 0.5f;
        }
        
        public override void Initialize(float damage, Item owner)
        {
            base.Initialize(damage, owner);
            _isInitialized = true;
            StartCoroutine(RotateProjectile());
        }
        
        private IEnumerator RotateProjectile()
        {
            while (gameObject.activeSelf)
            {
                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isInitialized) return;
            
            base.OnTriggerEnter2D(collision);
            
            /*if (collision.TryGetComponent(out Rigidbody2D enemyRb))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDirection * _knockbackForce, ForceMode2D.Impulse);
            }*/
            
            /*if (_hitEffect != null)
            {
                Instantiate(_hitEffect, collision.ClosestPoint(transform.position), Quaternion.identity);
            }*/
        }
    }
}