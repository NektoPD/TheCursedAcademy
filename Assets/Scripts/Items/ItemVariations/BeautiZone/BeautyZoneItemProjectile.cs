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
            Transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public void Activate()
        {
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
                damageable?.TakeDamage(Damage);
            }
        }
    }
}