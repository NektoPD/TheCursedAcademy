using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace EnemyLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyDamageView : MonoBehaviour
    {
        [SerializeField] private Color DamageColor = new(1f, 0.5f, 0.5f);

        [Header("Hit Impulse (Jump + Knockback)")]
        [SerializeField] private float impulseDuration = 0.12f;
        [SerializeField] private float knockbackDistance = 0.25f;
        [SerializeField] private float jumpHeight = 0.12f;
        [SerializeField] private Ease impulseEase = Ease.OutQuad;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _coroutine;

        private Tween _impulseTween;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
        }

        /// <summary>
        /// Просто флэш + небольшой импульс (если направление не важно).
        /// </summary>
        public void StartFlash(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            ApplyHitImpulse(transform.position + Vector3.left);
        }

        /// <summary>
        /// Флэш + подпрыгивание и отталкивание ОТ источника урона.
        /// hitFromWorldPos — позиция атакующего/точки удара в мире.
        /// </summary>
        public void StartFlash(float duration, Vector2 hitFromWorldPos)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            ApplyHitImpulse(hitFromWorldPos);
        }

        private void ApplyHitImpulse(Vector2 hitFromWorldPos)
        {
            _impulseTween?.Kill();

            Vector2 dir = ((Vector2)transform.position - hitFromWorldPos);
            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector2.right;

            dir.Normalize();

            float x = dir.x * knockbackDistance;
            float y = jumpHeight;

            Vector3 target = transform.position + new Vector3(x, y, 0f);

            _impulseTween = transform
                .DOMove(target, impulseDuration)
                .SetEase(impulseEase);
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            float halfDuration = duration / 2f;
            Color flashColor = DamageColor;

            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                _spriteRenderer.color = Color.Lerp(_originalColor, flashColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                _spriteRenderer.color = Color.Lerp(flashColor, _originalColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.color = _originalColor;
        }
    }
}
