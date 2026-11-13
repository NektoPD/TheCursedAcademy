using DG.Tweening;
using EnemyLogic;
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
        [SerializeField] private SpriteRenderer _projectileSprite;

        private float _speed;
        private float _lifetime;
        private EnemyDamageTaker _currentTarget;
        private Vector3 _targetScale;
        private Vector3 _hitEffectScale;
        private bool _isDespawning;
        private Tween _pulseTween;
        private Tween _lifetimeTween;

        protected override void Awake()
        {
            base.Awake();
            _targetScale = transform.localScale;

            if (_hitEffect != null)
            {
                _hitEffectScale = _hitEffect.transform.localScale;
                _hitEffect.SetActive(false);
            }
        }

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            StopAnimations();
            HitEnemies.Clear();
            _speed = speed * _speedMultiplier;
            _lifetime = Mathf.Max(0f, duration);
            _isDespawning = false;
            _currentTarget = null;

            if (_projectileSprite != null)
                _projectileSprite.color = new Color(_projectileSprite.color.r, _projectileSprite.color.g, _projectileSprite.color.b, 1f);

            base.Launch(direction, _speed, damage, duration);
            FindRandomTarget();

            float despawnDuration = Mathf.Min(_lifetime, Mathf.Max(0f, _despawnScaleDuration));
            float despawnDelay = _lifetime - despawnDuration;
            float spawnDuration = Mathf.Min(despawnDelay, Mathf.Max(0f, _spawnScaleDuration));

            if (spawnDuration > 0f)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(_targetScale, spawnDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(StartPulse);
            }
            else
            {
                transform.localScale = _targetScale;
            }

            if (despawnDelay > 0f)
                _lifetimeTween = DOVirtual.DelayedCall(despawnDelay, StartDespawn, false);
            else
                StartDespawn();
        }

        private void StartDespawn()
        {
            if (_isDespawning)
                return;

            _isDespawning = true;
            _pulseTween?.Kill();
            transform.DOKill();

            float duration = Mathf.Min(_lifetime, Mathf.Max(0f, _despawnScaleDuration));
            if (duration <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }

        private void StartPulse()
        {
            if (_isDespawning)
                return;

            _pulseTween = transform.DOScale(_targetScale * (1f + _pulseStrength), _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void Update()
        {
            if (_isDespawning)
                return;

            if (_currentTarget == null || _currentTarget.IsDied || !_currentTarget.gameObject.activeInHierarchy)
            {
                FindRandomTarget();
                if (_currentTarget == null)
                    return;
            }

            Vector2 dir = (Vector2)_currentTarget.transform.position - (Vector2)transform.position;
            if (dir.sqrMagnitude <= Mathf.Epsilon)
                return;

            dir.Normalize();
            Rb.velocity = dir * _speed;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isDespawning || !isActiveAndEnabled || (_enemyLayer.value & (1 << collision.gameObject.layer)) == 0)
                return;

            EnemyDamageTaker target = collision.GetComponentInParent<EnemyDamageTaker>();
            if (target == null || target.IsDied || !target.gameObject.activeInHierarchy || !HitEnemies.Add(target))
                return;

            ApplyEffect(target);

            if (_currentTarget == target)
                _currentTarget = null;
        }

        protected override void ApplyEffect(IDamageable target)
        {
            base.ApplyEffect(target);
            PlayHitEffect();
        }

        private void PlayHitEffect()
        {
            if (_hitEffect == null)
                return;

            _hitEffect.transform.DOKill();
            _hitEffect.SetActive(false);
            _hitEffect.transform.localScale = Vector3.zero;
            _hitEffect.SetActive(true);

            _hitEffect.transform.DOScale(_hitEffectScale, _hitEffectAppearDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _hitEffect.transform.DOScale(Vector3.zero, _hitEffectDisappearDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => _hitEffect.SetActive(false));
                });
        }

        private void StopAnimations()
        {
            _lifetimeTween?.Kill();
            _lifetimeTween = null;
            _pulseTween?.Kill();
            _pulseTween = null;
            transform.DOKill();

            if (_hitEffect != null)
            {
                _hitEffect.transform.DOKill();
                _hitEffect.SetActive(false);
                _hitEffect.transform.localScale = _hitEffectScale;
            }
        }

        private void OnDisable()
        {
            _isDespawning = true;
            StopAnimations();
            HitEnemies.Clear();
            _currentTarget = null;
            Rb.velocity = Vector2.zero;
            transform.localScale = _targetScale;
        }

        private void OnDestroy()
        {
            StopAnimations();
        }

        private void FindRandomTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRadius, _enemyLayer);

            _currentTarget = null;
            if (hits.Length == 0)
                return;

            int startIndex = Random.Range(0, hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                EnemyDamageTaker candidate = hits[(startIndex + i) % hits.Length].GetComponentInParent<EnemyDamageTaker>();
                if (candidate == null || candidate.IsDied || !candidate.gameObject.activeInHierarchy || HitEnemies.Contains(candidate))
                    continue;

                _currentTarget = candidate;
                return;
            }
        }
    }
}
