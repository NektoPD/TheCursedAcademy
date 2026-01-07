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

        [Header("Impulse Limits")]
        [SerializeField] private float impulseCooldown = 0.35f;          // не чаще, чем раз в N сек
        [SerializeField] private float minDistanceForFullKnockback = 0.8f; // если ближе — отталкивание режем
        [SerializeField] private float minKnockbackMultiplier = 0.15f;     // минимум (чтобы было хоть что-то)
        [SerializeField] private bool disableHorizontalKnockbackWhenVeryClose = true;
        [SerializeField] private float veryCloseDistance = 0.25f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _coroutine;

        private Tween _impulseTween;

        private float _nextImpulseTime; // кулдаун

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

        public void StartFlash(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));
        }

        public void StartFlash(float duration, Vector2 hitFromWorldPos)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            ApplyHitImpulse(hitFromWorldPos);
        }

        private void ApplyHitImpulse(Vector2 hitFromWorldPos)
        {
            // 1) КУЛДАУН: флеш можем показывать всегда, но импульс — не всегда
            if (Time.time < _nextImpulseTime)
                return;

            _nextImpulseTime = Time.time + impulseCooldown;

            _impulseTween?.Kill();

            Vector2 pos = transform.position;
            Vector2 dir = (pos - hitFromWorldPos);

            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector2.right;

            float dist = dir.magnitude;
            dir.Normalize();

            // 2) РЕЖЕМ ОТТАЛКИВАНИЕ, если враг близко к источнику удара (обычно игрок)
            float knockbackMul = 1f;
            if (minDistanceForFullKnockback > 0.0001f)
            {
                // dist=0 -> mul=minKnockbackMultiplier, dist>=minDistanceForFullKnockback -> mul=1
                knockbackMul = Mathf.Lerp(minKnockbackMultiplier, 1f, dist / minDistanceForFullKnockback);
                knockbackMul = Mathf.Clamp01(knockbackMul);
                knockbackMul = Mathf.Max(knockbackMul, minKnockbackMultiplier);
            }

            float x = dir.x * knockbackDistance * knockbackMul;
            float y = jumpHeight;

            // 3) Если совсем "в упоре" — можно вообще отключить горизонтальное отталкивание,
            // оставив только "подпрыгивание", чтобы не ломать милишников.
            if (disableHorizontalKnockbackWhenVeryClose && dist <= veryCloseDistance)
                x = 0f;

            Vector3 target = transform.position + new Vector3(x, y, 0f);

            _impulseTween = transform
                .DOMove(target, impulseDuration)
                .SetEase(impulseEase)
                .SetUpdate(false);
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
