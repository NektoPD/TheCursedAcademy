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
        [SerializeField] private float _pulseStrength = 0.15f;
        [SerializeField] private int _pulseVibrato = 6;

        private static readonly int ExplosionTrigger = Animator.StringToHash("Explosion");

        private Vector3 _targetScale;
        private bool _hasExploded;
        private Tween _scaleTween;
        private Tween _pulseTween;
        private Tween _flightTween;

        public event Action<CherryBombsProjectile> Finished;

        protected override void Awake()
        {
            base.Awake();
            _targetScale = Transform.localScale;
            if (_animator == null)
                _animator = GetComponent<Animator>();
        }

        public void Launch(Vector2 startPos, Vector2 direction, float speed)
        {
            _hasExploded = false;
            Transform.position = startPos;
            Transform.localScale = Vector3.zero;

            float distance = speed * _flightDuration;
            Vector2 endPos = startPos + direction * distance;

            KillTweens();

            _scaleTween = Transform.DOScale(_targetScale, _flightDuration).SetEase(Ease.OutBack);

            _flightTween = DOTween.To(
                () => 0f,
                t =>
                {
                    Vector2 linear = Vector2.Lerp(startPos, endPos, t);
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
            Transform.localScale = _targetScale;
            Transform.rotation = Quaternion.identity;
            _hasExploded = false;
            Damage = 0f;
            Owner = null;
        }

        private void KillTweens()
        {
            _scaleTween?.Kill();
            _pulseTween?.Kill();
            _flightTween?.Kill();
            _scaleTween = null;
            _pulseTween = null;
            _flightTween = null;
        }

        private void Explode()
        {
            if (_hasExploded) return;
            _hasExploded = true;

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

            Finished?.Invoke(this);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
        }
    }
}
