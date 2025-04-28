using UnityEngine;
using HealthSystem;
using Items.BaseClass;

namespace Items.ItemVariations
{
    public class GreatIdeaProjectile : ItemProjectile
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _hitAnimationTrigger = "Hit";
        
        private Vector2 _direction;
        private float _speed;
        private bool _isMoving = false;
        
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction, float speed)
        {
            _direction = direction;
            _speed = speed;
            _isMoving = true;
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage);
                
                // Play hit animation if animator exists
                if (_animator != null)
                {
                    _animator.SetTrigger(_hitAnimationTrigger);
                }
                
                // Optional: Stop moving after hit
                // _isMoving = false;
            }
        }

        private void OnDisable()
        {
            // Reset state when returned to pool
            _isMoving = false;
            
            // Reset trail if it exists
            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
            }
        }
    }
}