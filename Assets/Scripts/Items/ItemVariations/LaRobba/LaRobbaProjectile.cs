using System;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.LaRobba
{
    public class LaRobbaProjectile : ItemProjectile
    {
        private enum Phase
        {
            Falling,
            Bouncing
        }

        [SerializeField] private Sprite[] _sprites;

        private Vector2 _direction;
        private float _speed;
        private float _targetY;
        private Phase _phase;
        private CircleCollider2D _circleCollider;
        private bool _hitTarget;

        public event Action<LaRobbaProjectile> Finished;

        protected override void Awake()
        {
            base.Awake();
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        public void Launch(Vector2 targetPosition, float speed)
        {
            _speed = speed;
            _targetY = targetPosition.y;
            _phase = Phase.Falling;
            _direction = Vector2.down;
            _hitTarget = false;

            if (_sprites != null && _sprites.Length > 0)
            {
                Sprite randomSprite = _sprites[UnityEngine.Random.Range(0, _sprites.Length)];
                SpriteRenderer.sprite = randomSprite;
                UpdateColliderToSprite(randomSprite);
            }

            Transform.rotation = Quaternion.identity;
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
            _hitTarget = false;
            ClearHitEnemies();
        }

        private void Update()
        {
            Transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));

            if (_phase == Phase.Falling)
            {
                if (Transform.position.y <= _targetY && !_hitTarget)
                {
                    _hitTarget = true;
                    _phase = Phase.Bouncing;
                    _direction = Vector2.down;
                    ClearHitEnemies();
                }
            }
            else
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
                damageable.TakeDamage(Damage);
                Owner?.RaiseDamageDealt(Damage);

                if (_phase == Phase.Falling)
                {
                    _hitTarget = true;
                    _phase = Phase.Bouncing;
                    _direction = Vector2.down;
                    ClearHitEnemies();
                }
            }
        }
    }
}
