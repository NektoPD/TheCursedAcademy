using EnemyLogic;
using HealthSystem;
using UnityEngine;
using Utils;

namespace CharacterLogic.Abilities
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer), typeof(SimpleSpriteAnimator))]
    public class PoisonProjectile : AbilityProjectile
    {
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private SpriteRenderer _projectileSprite;
        [SerializeField, Range(0f, 1f)] private float _areaRadiusMultiplier = 0.73f;

        private CircleCollider2D _areaCollider;
        private SimpleSpriteAnimator _spriteAnimator;
        private float _poisonDuration;

        protected override void Awake()
        {
            base.Awake();
            _areaCollider = GetComponent<CircleCollider2D>();
            _areaCollider.enabled = false;
            _spriteAnimator = GetComponent<SimpleSpriteAnimator>();

            if (_projectileSprite == null)
                _projectileSprite = GetComponent<SpriteRenderer>();

            Rb.bodyType = RigidbodyType2D.Kinematic;
            Rb.simulated = false;
        }

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            base.Launch(Vector2.zero, 0f, damage, duration);
            _poisonDuration = duration;
            HitEnemies.Clear();
            UpdateAreaFromSprite();
            _spriteAnimator.Finished += OnAnimationFinished;
            DamageArea();
        }

        private void UpdateAreaFromSprite()
        {
            Sprite sprite = _projectileSprite.sprite;
            if (sprite == null)
                return;

            Vector2 size = sprite.rect.size / sprite.pixelsPerUnit;
            _areaCollider.radius = Mathf.Min(size.x, size.y) * 0.5f * _areaRadiusMultiplier;
            _areaCollider.offset = (sprite.rect.size * 0.5f - sprite.pivot) / sprite.pixelsPerUnit;
        }

        private void DamageArea()
        {
            Vector3 scale = transform.lossyScale;
            float radius = _areaCollider.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
            Vector2 center = transform.TransformPoint(_areaCollider.offset);
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, _enemyLayer);

            foreach (Collider2D hit in hits)
            {
                EnemyDamageTaker target = hit.GetComponentInParent<EnemyDamageTaker>();
                if (target == null || target.IsDied || !HitEnemies.Add(target))
                    continue;

                ApplyEffect(target);
            }
        }

        protected override void ApplyEffect(IDamageable target)
        {
            base.ApplyEffect(target);

            if (target is not Component component || component == null ||
                !component.gameObject.activeInHierarchy || target is EnemyDamageTaker { IsDied: true })
                return;

            PoisonEffect poison = component.GetComponent<PoisonEffect>();
            if (poison == null)
                poison = component.gameObject.AddComponent<PoisonEffect>();

            poison.Apply(target, Damage, _poisonDuration);
        }

        private void OnAnimationFinished()
        {
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            if (_spriteAnimator != null)
                _spriteAnimator.Finished -= OnAnimationFinished;
        }
    }
}
