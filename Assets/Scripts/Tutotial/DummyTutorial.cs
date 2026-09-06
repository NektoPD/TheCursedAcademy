using System;
using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class DummyTutorial : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int _hitsToComplete = 5;
        [SerializeField, Min(1f)] private float _hitScale = 1.12f;
        [SerializeField, Min(0.01f)] private float _hitDuration = 0.12f;
        [SerializeField] private Color _hitColor = new(1f, 0.55f, 0.55f, 1f);

        private SpriteRenderer _spriteRenderer;
        private Vector3 _initialScale;
        private Color _initialColor;
        private Sequence _hitSequence;
        private int _hitCount;
        private bool _completed;

        public bool IsDied => false;

        public event Action HitsCompleted;
        public event Action<int, int> HitRegistered;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _initialScale = transform.localScale;
            _initialColor = _spriteRenderer.color;
        }

        private void OnDisable()
        {
            _hitSequence?.Kill();
            transform.localScale = _initialScale;

            if (_spriteRenderer != null)
                _spriteRenderer.color = _initialColor;
        }

        public void TakeDamage(float damage, bool isFromBerserk = false)
        {
            PlayHitFeedback();

            if (_completed)
                return;

            _hitCount++;
            HitRegistered?.Invoke(_hitCount, _hitsToComplete);

            if (_hitCount < _hitsToComplete)
                return;

            _completed = true;
            HitsCompleted?.Invoke();
        }

        private void PlayHitFeedback()
        {
            _hitSequence?.Kill();
            transform.localScale = _initialScale;
            _spriteRenderer.color = _initialColor;

            _hitSequence = DOTween.Sequence()
                .Append(transform.DOScale(_initialScale * _hitScale, _hitDuration).SetEase(Ease.OutQuad))
                .Join(_spriteRenderer.DOColor(_hitColor, _hitDuration))
                .Append(transform.DOScale(_initialScale, _hitDuration).SetEase(Ease.InQuad))
                .Join(_spriteRenderer.DOColor(_initialColor, _hitDuration));
        }
    }
}
