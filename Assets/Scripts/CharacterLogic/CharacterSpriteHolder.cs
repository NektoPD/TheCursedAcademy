using System.Collections;
using UnityEngine;

namespace CharacterLogic
{
    public class CharacterSpriteHolder : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _pulseSpeed = 1f;
        [SerializeField] private float _pulseIntensity = 1f;
        [SerializeField] private Color _targetColor;
        [SerializeField] private float _invincibleAlpha = 0.4f;

        [Header("Death Visuals")]
        [SerializeField] private float _deathToRedDelay = 0.05f;
        [SerializeField] private float _deathFadeDuration = 0.7f;

        private MaterialPropertyBlock _materialPropertyBlock;
        private Coroutine _colorChangeCoroutine;
        private Color _originalColor;
        private bool _isPulsating;

        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            _originalColor = _spriteRenderer.color;
        }

        public void FlipSprite(bool status)
        {
            _spriteRenderer.flipX = status;
        }

        public void StartPulsing()
        {
            if (_colorChangeCoroutine != null)
                StopCoroutine(_colorChangeCoroutine);

            _isPulsating = true;
            _colorChangeCoroutine = StartCoroutine(PulseColorRoutine());
        }

        public void StopPulsing()
        {
            StopColorCoroutine();

            _isPulsating = false;
            ApplyColor(_originalColor);
        }

        public void SetInvincibleVisual(bool enabled)
        {
            Color color = _originalColor;
            color.a = enabled ? _invincibleAlpha : 1f;
            ApplyColor(color);
        }

        /// <summary>
        /// Запускает визуал смерти: цвет -> красный, затем alpha -> 0 (unscaled time).
        /// </summary>
        public Coroutine PlayDeathFade(MonoBehaviour runner)
        {
            StopColorCoroutine();
            _isPulsating = false;

            _colorChangeCoroutine = runner.StartCoroutine(DeathFadeRoutine());
            return _colorChangeCoroutine;
        }

        /// <summary>
        /// Полный сброс к оригинальному цвету (полезно при revive/respawn).
        /// </summary>
        public void ResetVisual()
        {
            StopColorCoroutine();
            _isPulsating = false;

            Color c = _originalColor;
            c.a = 1f;
            ApplyColor(c);
        }

        private IEnumerator PulseColorRoutine()
        {
            while (_isPulsating)
            {
                float t = Mathf.PingPong(Time.time * _pulseSpeed, 1f) * _pulseIntensity;
                Color lerpedColor = Color.Lerp(_originalColor, _targetColor, t);
                ApplyColor(lerpedColor);
                yield return null;
            }
        }

        private IEnumerator DeathFadeRoutine()
        {
            Color red = Color.red;
            red.a = 1f;
            ApplyColor(red);

            float wait = Mathf.Max(0f, _deathToRedDelay);
            float w = 0f;
            while (w < wait)
            {
                w += Time.unscaledDeltaTime;
                yield return null;
            }

            float t = 0f;
            float dur = Mathf.Max(0.0001f, _deathFadeDuration);
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / dur);

                Color c = Color.red;
                c.a = Mathf.Lerp(1f, 0f, k);
                ApplyColor(c);

                yield return null;
            }

            Color end = Color.red;
            end.a = 0f;
            ApplyColor(end);

            _colorChangeCoroutine = null;
        }

        private void StopColorCoroutine()
        {
            if (_colorChangeCoroutine != null)
            {
                StopCoroutine(_colorChangeCoroutine);
                _colorChangeCoroutine = null;
            }
        }

        private void ApplyColor(Color color)
        {
            _spriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_Color", color);
            _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
