using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace EnemyLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyDamageView : MonoBehaviour
    {
        [SerializeField] private Color DamageColor = new(1f, 0.5f, 0.5f);

        [Header("Shake Settings")]
        private float shakeDuration = 0.15f;
         private float shakeStrength = 0.15f;
         private int shakeVibrato = 15;
         private float shakeRandomness = 90f;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _coroutine;

        private Vector3 _originalPosition;
        private Tween _shakeTween;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _originalColor = _spriteRenderer.color;
            _originalPosition = transform.localPosition;
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _spriteRenderer.color = _originalColor;

            _shakeTween?.Kill();
            transform.localPosition = _originalPosition;
        }

        public void StartFlash(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(duration));

            PlayShake();
        }

        private void PlayShake()
        {
            _shakeTween?.Kill();

            _shakeTween = transform.DOShakePosition(
                shakeDuration,
                shakeStrength,
                shakeVibrato,
                shakeRandomness,
                snapping: false,
                fadeOut: false
            )/*.OnComplete(() =>
            {
                transform.localPosition = _originalPosition;
            })*/;
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
