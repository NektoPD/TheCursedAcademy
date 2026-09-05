using System;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.LaRobba
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LaRobbaProjectile : ItemProjectile
    {
        private enum Phase
        {
            Falling,
            Bouncing
        }

        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private float _gravity = 5f;
        [SerializeField] private float _bounceForce = 3f;

        private float _speed;
        private Phase _phase;
        private CircleCollider2D _circleCollider;
        private Rigidbody2D _rb;
        private Sprite _defaultSprite;
        private float _defaultColliderRadius;

        public event Action<LaRobbaProjectile> Finished;

        protected override void Awake()
        {
            base.Awake();
            _circleCollider = GetComponent<CircleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
            _rb.freezeRotation = false;
            _rb.constraints = RigidbodyConstraints2D.None;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _defaultSprite = SpriteRenderer.sprite;
            _defaultColliderRadius = _circleCollider != null ? _circleCollider.radius : 0.5f;
        }

        public void Launch(Vector2 targetPosition, float speed)
        {
            _speed = speed;
            _phase = Phase.Falling;

            if (_sprites != null && _sprites.Length > 0)
            {
                Sprite randomSprite = _sprites[UnityEngine.Random.Range(0, _sprites.Length)];
                SpriteRenderer.sprite = randomSprite;
                UpdateColliderToSprite(randomSprite);
            }

            _circleCollider.enabled = false;
            
            Transform.rotation = Quaternion.identity;
            _rb.velocity = Vector2.down * _speed;
            _rb.gravityScale = _gravity / Physics2D.gravity.magnitude;
        }

        private void UpdateColliderToSprite(Sprite sprite)
        {
            if (_circleCollider == null || sprite == null) return;
            float maxSide = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
            _circleCollider.radius = maxSide * 0.5f;
        }

        private void OnEnable()
        {
            _phase = Phase.Falling;
            ClearHitEnemies();
        }

        private void OnDisable()
        {
            ResetToDefault();
        }

        private void ResetToDefault()
        {
            if (_rb != null)
            {
                _rb.velocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.gravityScale = 0f;
            }

            Transform.rotation = Quaternion.identity;

            if (SpriteRenderer != null && _defaultSprite != null)
                SpriteRenderer.sprite = _defaultSprite;

            if (_circleCollider != null)
            {
                _circleCollider.radius = _defaultColliderRadius;
                _circleCollider.enabled = true;
            }

            _phase = Phase.Falling;
            Damage = 0f;
            Owner = null;
        }

        private void Update()
        {
            if (!_circleCollider.enabled)
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    Vector3 viewportPos = camera.WorldToViewportPoint(Transform.position);
                    if (viewportPos.y <= 1f && viewportPos.y >= 0f)
                        _circleCollider.enabled = true;
                }
            }

            if (_phase == Phase.Bouncing)
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    float bottomEdge = camera.ViewportToWorldPoint(Vector3.zero).y;
                    if (Transform.position.y < bottomEdge - 1f)
                    {
                        Finished?.Invoke(this);
                    }
                }
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                HitEnemies.Add(damageable))
            {
                damageable.TakeDamage(Damage, IsBerserkDamage);
                Owner?.RaiseDamageDealt(Damage);

                if (_phase == Phase.Falling)
                {
                    _phase = Phase.Bouncing;
                    ClearHitEnemies();

                    Vector2 bounceDir = new Vector2(
                        UnityEngine.Random.Range(-0.2f, 0.2f),
                        1f
                    ).normalized;
                    _rb.velocity = bounceDir * _bounceForce;
                    _rb.angularVelocity = UnityEngine.Random.Range(-360f, 360f);
                }
            }
        }
    }
}
