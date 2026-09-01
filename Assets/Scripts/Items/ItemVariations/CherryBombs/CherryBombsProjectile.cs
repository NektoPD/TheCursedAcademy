using System;
using System.Collections.Generic;
using CharacterLogic;
using DG.Tweening;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.CherryBombs
{
    public class CherryBombsProjectile : ItemProjectile
    {
        [SerializeField] private float _explosionRadius = 1.5f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _flightDuration = 0.8f;
        [SerializeField] private float _arcHeight = 3f;
        [SerializeField] private float _explosionDelay = 1f;
        [SerializeField] private float _explosionEffectDuration = 0.5f;
        [SerializeField] private float _pulseStrength = 0.15f;
        [SerializeField] private int _pulseVibrato = 6;

        private static readonly int ExplosionTrigger = Animator.StringToHash("Explosion");
        private static readonly int ExplosionEffectTrigger = Animator.StringToHash("ExplosionEffect");

        private Vector3 _targetScale;
        private Transform _target;
        private Vector3 _targetPosition;
        private bool _hasExploded;
        private Tween _scaleTween;
        private Tween _pulseTween;
        private Tween _flightTween;
        private Tween _delayedFinish;

        public event Action<CherryBombsProjectile> Finished;

        protected override void Awake()
        {
            base.Awake();
            _targetScale = Transform.localScale;
            if (_animator == null)
                _animator = GetComponent<Animator>();
        }

        public void Launch(Vector2 startPos, Vector2 direction, float speed, Transform target = null)
        {
            _hasExploded = false;
            Transform.position = startPos;
            Transform.localScale = Vector3.zero;

            _target = target;
            _targetPosition = startPos + direction * (speed * _flightDuration);

            KillTweens();

            _scaleTween = Transform.DOScale(_targetScale, _flightDuration).SetEase(Ease.OutBack);

            _flightTween = DOTween.To(
                () => 0f,
                t =>
                {
                    if (_target != null)
                        _targetPosition = _target.position;

                    Vector2 linear = Vector2.Lerp(startPos, _targetPosition, t);
                    float arc = _arcHeight * 4f * t * (1f - t);
                    Transform.position = new Vector3(linear.x, linear.y + arc, 0f);
                },
                1f,
                _flightDuration
            ).SetEase(Ease.Linear).OnComplete(OnFlightComplete);
        }

        private void OnFlightComplete()
        {
            if (_animator != null)
                _animator.SetTrigger(ExplosionTrigger);

            _pulseTween = Transform.DOPunchScale(
                Vector3.one * _pulseStrength,
                _explosionDelay,
                _pulseVibrato
            );

            DOVirtual.DelayedCall(_explosionDelay, Explode);
        }

        private void OnEnable()
        {
            _hasExploded = false;
            ClearHitEnemies();
        }

        private void OnDisable()
        {
            ResetToDefault();
        }

        private void ResetToDefault()
        {
            KillTweens();

            if (_animator != null)
            {
                _animator.ResetTrigger(ExplosionTrigger);
                _animator.ResetTrigger(ExplosionEffectTrigger);
                _animator.Rebind();
                _animator.Update(0f);
            }

            Transform.localScale = _targetScale;
            Transform.rotation = Quaternion.identity;
            _target = null;
            _hasExploded = false;
            Damage = 0f;
            Owner = null;
        }

        private void KillTweens()
        {
            _scaleTween?.Kill();
            _pulseTween?.Kill();
            _flightTween?.Kill();
            _delayedFinish?.Kill();
            _scaleTween = null;
            _pulseTween = null;
            _flightTween = null;
            _delayedFinish = null;
        }

        private void Explode()
        {
            if (_hasExploded) return;
            _hasExploded = true;

            if (_animator != null)
                _animator.SetTrigger(ExplosionEffectTrigger);

            _delayedFinish = DOVirtual.DelayedCall(_explosionEffectDuration, () => Finished?.Invoke(this));
        }

        public void DealExplosionDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(Transform.position, _explosionRadius, _enemyLayer);
            var damaged = new HashSet<IDamageable>();

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out IDamageable damageable) ||
                    hit.TryGetComponent(out Character character) ||
                    !damaged.Add(damageable))
                    continue;

                damageable.TakeDamage(Damage);
                Owner?.RaiseDamageDealt(Damage);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
        }
    }
}
