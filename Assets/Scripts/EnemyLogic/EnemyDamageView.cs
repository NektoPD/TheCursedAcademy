using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace EnemyLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyDamageView : MonoBehaviour
    {
        [SerializeField] private Color _damageColor = new(1f, 0.5f, 0.5f);
        [SerializeField, Range(0f, 1f)] private float _flashHoldPart = 0.35f;
        
        [Header("Hit Impulse (Jump + Knockback)")]
        [SerializeField] private float _impulseDuration = 0.12f;
        [SerializeField] private float _knockbackDistance = 0.25f;
        [SerializeField] private float _jumpHeight = 0.12f;
        [SerializeField] private Ease _impulseEase = Ease.OutQuad;

        [Header("Impulse Limits")]
        [SerializeField] private float _impulseCooldown = 0.35f;

        [SerializeField] private float _minDistanceForFullKnockback = 0.8f;
        [SerializeField] private float _minKnockbackMultiplier = 0.15f;
        [SerializeField] private bool _disableHorizontalKnockbackWhenVeryClose = true;
        [SerializeField] private float _veryCloseDistance = 0.25f;

        [SerializeField] private float _impulseCooldownNormal = 0.35f;
        [SerializeField] private float _impulseCooldownBoss = 0.7f;

        [Header("Hit Squash & Stretch")]
        [SerializeField] private bool _useSquash = true;
        [SerializeField] private float _squashAmount = 0.18f;
        [SerializeField] private float _squashDuration = 0.16f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _coroutine;

        private Tween _impulseTween;
        private Tween _squashTween;
        private Vector3 _originalScale;

        private float _nextImpulseTime;
        private int _bossEnemyId = 1000;
        
        public int EnemyId { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalScale = transform.localScale;
        }

        private void Start()
        {
            _originalColor = _spriteRenderer.color;
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _spriteRenderer.color = _originalColor;

            _impulseTween?.Kill();

            _squashTween?.Kill();
            transform.localScale = _originalScale;
        }

        public void Initialize(int enemyId)
        {
            EnemyId = enemyId;

            _impulseCooldown = EnemyId >= _bossEnemyId ? _impulseCooldownBoss : _impulseCooldownNormal;
        }

        public void StartFlash(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            ApplySquash();
        }

        public void StartFlash(float duration, Vector2 hitFromWorldPos)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            ApplySquash();
            ApplyHitImpulse(hitFromWorldPos);
        }

        private void ApplySquash()
        {
            if (_useSquash == false)
                return;

            _squashTween?.Kill();
            transform.localScale = _originalScale;

            Vector3 squashed = new Vector3(
                _originalScale.x * (1f + _squashAmount),
                _originalScale.y * (1f - _squashAmount),
                _originalScale.z);

            _squashTween = DOTween.Sequence()
                .Append(transform.DOScale(squashed, _squashDuration * 0.35f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(_originalScale, _squashDuration * 0.65f).SetEase(Ease.OutBack));
        }

        private void ApplyHitImpulse(Vector2 hitFromWorldPos)
        {
            if (Time.time < _nextImpulseTime)
                return;

            _nextImpulseTime = Time.time + _impulseCooldown;

            _impulseTween?.Kill();

            Vector2 pos = transform.position;
            Vector2 dir = (pos - hitFromWorldPos);

            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector2.right;

            float dist = dir.magnitude;
            dir.Normalize();

            float knockbackMul = 1f;
            if (_minDistanceForFullKnockback > 0.0001f)
            {
                knockbackMul = Mathf.Lerp(_minKnockbackMultiplier, 1f, dist / _minDistanceForFullKnockback);
                knockbackMul = Mathf.Clamp01(knockbackMul);
                knockbackMul = Mathf.Max(knockbackMul, _minKnockbackMultiplier);
            }

            float x = dir.x * _knockbackDistance * knockbackMul;
            float y = _jumpHeight;

            if (_disableHorizontalKnockbackWhenVeryClose && dist <= _veryCloseDistance)
                x = 0f;

            Vector3 target = transform.position + new Vector3(x, y, 0f);

            _impulseTween = transform
                .DOMove(target, _impulseDuration)
                .SetEase(_impulseEase)
                .SetUpdate(false);
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            _spriteRenderer.color = _damageColor;

            float holdTime = duration * _flashHoldPart;
            yield return new WaitForSeconds(holdTime);

            float fadeDuration = Mathf.Max(0.0001f, duration - holdTime);
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                _spriteRenderer.color = Color.Lerp(_damageColor, _originalColor, t * t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.color = _originalColor;
        }
    }
}
