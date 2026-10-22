using System.Collections;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.BeautiZone
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BeautyZoneItemProjectile : ItemProjectile
    {
        [SerializeField] private float _damageTickInterval = 0.5f;
        [SerializeField] private SpriteRenderer _effectRenderer;
        [SerializeField, Min(0f)] private float _effectScaleMultiplier = 1f;

        private CircleCollider2D _circleCollider;
        private float _duration;
        private bool _isActive = false;
        private Coroutine _damageTickCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _circleCollider = GetComponent<CircleCollider2D>();
            _circleCollider.isTrigger = true;
            _circleCollider.enabled = false;
        }

        public override void Initialize(float damage, Item owner)
        {
            base.Initialize(damage, owner);
            ClearHitEnemies();
        }

        public void SetRadius(float radius)
        {
            Transform.localScale = new Vector3(radius, radius);
            _circleCollider.radius = radius;
            UpdateEffectScale();
        }

        private void UpdateEffectScale()
        {
            if (_effectRenderer == null || _effectRenderer.sprite == null)
                return;

            Vector2 spriteSize = _effectRenderer.sprite.rect.size / _effectRenderer.sprite.pixelsPerUnit;
            Transform effectTransform = _effectRenderer.transform;
            Vector3 parentScale = effectTransform.parent != null
                ? effectTransform.parent.lossyScale
                : Vector3.one;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f
                || Mathf.Approximately(parentScale.x, 0f) || Mathf.Approximately(parentScale.y, 0f))
                return;

            Vector3 zoneScale = Transform.lossyScale;
            float diameter = _circleCollider.radius * 2f * _effectScaleMultiplier
                * Mathf.Max(Mathf.Abs(zoneScale.x), Mathf.Abs(zoneScale.y));
            Vector3 effectScale = effectTransform.localScale;
            effectScale.x = diameter / (spriteSize.x * Mathf.Abs(parentScale.x));
            effectScale.y = diameter / (spriteSize.y * Mathf.Abs(parentScale.y));
            effectTransform.localScale = effectScale;
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public void Activate()
        {
            if (_effectRenderer != null)
            {
                _effectRenderer.gameObject.SetActive(true);
                UpdateEffectScale();
            }

            _isActive = true;
            _circleCollider.enabled = true;

            if (_damageTickCoroutine != null)
            {
                StopCoroutine(_damageTickCoroutine);
            }

            _damageTickCoroutine = StartCoroutine(DamageTick());

            StartCoroutine(DeactivateAfterDuration());
        }

        private IEnumerator DeactivateAfterDuration()
        {
            yield return new WaitForSeconds(_duration);

            _isActive = false;
            _circleCollider.enabled = false;

            if (_damageTickCoroutine != null)
            {
                StopCoroutine(_damageTickCoroutine);
                _damageTickCoroutine = null;
            }
        }

        private IEnumerator DamageTick()
        {
            WaitForSeconds waitInterval = new WaitForSeconds(_damageTickInterval);

            while (_isActive)
            {
                yield return waitInterval;

                ClearHitEnemies();
            }
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            if (!_isActive) return;

            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out CharacterLogic.Character character) &&
                !HitEnemies.Contains(damageable))
            {
                HitEnemies.Add(damageable);
                DealDamage(damageable);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}