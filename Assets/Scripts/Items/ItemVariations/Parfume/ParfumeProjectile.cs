using System;
using System.Collections;
using UnityEngine;
using HealthSystem;
using Items.BaseClass;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(Animator))]
    public class ParfumeProjectile : ItemProjectile
    {
        [SerializeField] private Sprite _flyingSprite;
        [SerializeField] private Sprite _damageZoneSprite;
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private string _explosionAnimTrigger = "Explosion";

        private Animator _animator;
        private Transform _transform;
        private Vector2 _targetPosition;
        private float _speed;
        private float _damageZoneDuration;
        private bool _isMoving = false;
        private bool _reachedTarget = false;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _transform = transform;

            if (_collider == null)
            {
                _collider = GetComponent<CircleCollider2D>();
            }

            _collider.enabled = false;
        }

        public override void Initialize(float damage, Item owner)
        {
            base.Initialize(damage, owner);
            _reachedTarget = false;
            _isMoving = false;

            SpriteRenderer.sprite = _flyingSprite;
        }


        public void SetupMovement(Vector2 targetPosition, float speed, float damageZoneDuration)
        {
            SpriteRenderer.sprite = _flyingSprite;

            _targetPosition = targetPosition;
            _speed = speed;
            _damageZoneDuration = damageZoneDuration;
            _isMoving = true;
        }

        private void Update()
        {
            if (_isMoving && !_reachedTarget)
            {
                _transform.position = Vector2.MoveTowards(
                    _transform.position,
                    _targetPosition,
                    _speed * Time.deltaTime
                );

                if (Vector2.Distance(_transform.position, _targetPosition) < 0.1f)
                {
                    ReachTarget();
                }
            }
        }

        private void ReachTarget()
        {
            _reachedTarget = true;
            _isMoving = false;

            SpriteRenderer.sprite = _damageZoneSprite;
            _collider.enabled = true;

            if (_animator != null)
            {
                _animator.SetTrigger(_explosionAnimTrigger);
            }

            _transform.rotation = Quaternion.identity;

            StartCoroutine(DisableDamageZoneAfterDelay());
        }

        private IEnumerator DisableDamageZoneAfterDelay()
        {
            yield return new WaitForSeconds(_damageZoneDuration);

            _collider.enabled = false;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_reachedTarget) return;

            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage);
            }
        }
    }
}