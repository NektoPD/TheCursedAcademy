using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class FireblastProjectile : AbilityProjectile
    {
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private float _searchRadius = 30f;
        [SerializeField] private float _spawnScaleDuration = 0.3f;
        [SerializeField] private float _despawnScaleDuration = 0.3f;
        [SerializeField] private float _speedMultiplier = 0.4f;
        [SerializeField] private float _pulseStrength = 0.15f;
        [SerializeField] private float _pulseDuration = 0.35f;
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private float _hitEffectAppearDuration = 0.2f;
        [SerializeField] private float _hitEffectDisappearDuration = 0.3f;

        private float _speed;
        private float _lifetime;
        private Transform _currentTarget;
        private Vector3 _targetScale;
        private bool _isDespawning;
        private bool _isHit;
        private Tween _pulseTween;

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            _speed = speed * _speedMultiplier;
            _lifetime = duration;
            _targetScale = transform.localScale;
            _isDespawning = false;
            _isHit = false;

            if (_hitEffect != null)
                _hitEffect.SetActive(false);

            transform.localScale = Vector3.zero;
            transform.DOScale(_targetScale, _spawnScaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(StartPulse);

            base.Launch(direction, _speed, damage, duration);
            FindRandomTarget();

            float despawnDelay = _lifetime - _despawnScaleDuration;
            if (despawnDelay < 0f) despawnDelay = 0f;

            DOVirtual.DelayedCall(despawnDelay, StartDespawn);
        }

        private void StartDespawn()
        {
            if (this == null || _isHit) return;
            _isDespawning = true;
            _pulseTween?.Kill();
            transform.DOScale(Vector3.zero, _despawnScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }

        private void StartPulse()
        {
            if (_isDespawning || _isHit) return;
            _pulseTween = transform.DOScale(_targetScale * (1f + _pulseStrength), _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void Update()
        {
            if (_isDespawning || _isHit) return;

            if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
            {
                FindRandomTarget();
                if (_currentTarget == null) return;
            }

            Vector2 dir = ((Vector2)_currentTarget.position - (Vector2)transform.position).normalized;
            Rb.velocity = dir * _speed;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void ApplyEffect(IDamageable target)
        {
            base.ApplyEffect(target);
            PlayHitEffect();
        }

        private void PlayHitEffect()
        {
            _isHit = true;
            _pulseTween?.Kill();
            Rb.velocity = Vector2.zero;

            if (_hitEffect == null)
            {
                Destroy(gameObject);
                return;
            }

            _hitEffect.SetActive(true);
            Vector3 effectScale = _hitEffect.transform.localScale;
            _hitEffect.transform.localScale = Vector3.zero;

            _hitEffect.transform.DOScale(effectScale, _hitEffectAppearDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _hitEffect.transform.DOScale(Vector3.zero, _hitEffectDisappearDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => Destroy(gameObject));
                });
        }

        private void OnDestroy()
        {
            transform.DOKill();
            if (_hitEffect != null)
                _hitEffect.transform.DOKill();
        }

        private void FindRandomTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRadius, _enemyLayer);

            if (hits.Length == 0)
            {
                _currentTarget = null;
                return;
            }

            int startIndex = Random.Range(0, hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                var candidate = hits[(startIndex + i) % hits.Length];
                if (candidate.transform != _currentTarget)
                {
                    _currentTarget = candidate.transform;
                    return;
                }
            }

            _currentTarget = hits[startIndex].transform;
        }
    }
}
