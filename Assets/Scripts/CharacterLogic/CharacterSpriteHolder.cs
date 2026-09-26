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
        private Coroutine _reviveAppearCoroutine;
        private Coroutine _ringCoroutine;
        private LineRenderer _ring;
        private Material _ringMaterial;
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
        public Coroutine PlayDeathFade()
        {
            StopRing();
            StopReviveAppear();
            StopColorCoroutine();
            _isPulsating = false;

            _colorChangeCoroutine = StartCoroutine(DeathFadeRoutine());
            return _colorChangeCoroutine;
        }

        /// <summary>
        /// Полный сброс к оригинальному цвету (полезно при revive/respawn).
        /// </summary>
        public void ResetVisual()
        {
            StopRing();
            StopReviveAppear();
            StopColorCoroutine();
            _isPulsating = false;

            Color c = _originalColor;
            c.a = 1f;
            ApplyColor(c);
        }

        public void PlayReviveAppear()
        {
            StopReviveAppear();
            _reviveAppearCoroutine = StartCoroutine(ReviveAppearRoutine());
            PlayRing(new Color(0.35f, 1f, 0.7f), 0.6f);
        }

        public void PlayLevelUpFlash()
        {
            StopColorCoroutine();
            _colorChangeCoroutine = StartCoroutine(LevelFlashRoutine());
            PlayRing(new Color(1f, 0.85f, 0.3f), 0.45f);
        }

        private IEnumerator LevelFlashRoutine()
        {
            const float duration = 0.35f;
            float elapsed = 0f;
            Color flash = new Color(1f, 0.8f, 0.25f, _originalColor.a);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float strength = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
                ApplyColor(Color.Lerp(_originalColor, flash, strength));
                yield return null;
            }

            ApplyColor(_originalColor);
            _colorChangeCoroutine = _isPulsating ? StartCoroutine(PulseColorRoutine()) : null;
        }

        private void PlayRing(Color color, float duration)
        {
            StopRing();
            if (_ring == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null)
                    return;

                GameObject ringObject = new GameObject("FeedbackRing");
                ringObject.transform.SetParent(_spriteRenderer.transform, false);
                _ring = ringObject.AddComponent<LineRenderer>();
                _ringMaterial = new Material(shader);
                _ring.sharedMaterial = _ringMaterial;
                _ring.useWorldSpace = false;
                _ring.loop = true;
                _ring.positionCount = 40;
                _ring.widthMultiplier = 0.06f;
                _ring.sortingLayerID = _spriteRenderer.sortingLayerID;
                _ring.sortingOrder = _spriteRenderer.sortingOrder + 1;
            }

            _ring.enabled = true;
            _ringCoroutine = StartCoroutine(RingRoutine(color, duration));
        }

        private IEnumerator RingRoutine(Color color, float duration)
        {
            float elapsed = 0f;
            Vector3 center = _spriteRenderer.sprite != null ? _spriteRenderer.sprite.bounds.center : Vector3.zero;
            float size = _spriteRenderer.sprite != null
                ? _spriteRenderer.sprite.bounds.extents.magnitude : 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float radius = Mathf.Lerp(size * 0.4f, size * 1.5f, progress);
                Color tint = color;
                tint.a = 1f - progress;
                _ring.startColor = tint;
                _ring.endColor = tint;

                for (int i = 0; i < _ring.positionCount; i++)
                {
                    float angle = i * 2f * Mathf.PI / _ring.positionCount;
                    _ring.SetPosition(i, center + new Vector3(
                        Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
                }

                yield return null;
            }

            _ring.enabled = false;
            _ringCoroutine = null;
        }

        private void StopRing()
        {
            if (_ringCoroutine != null)
            {
                StopCoroutine(_ringCoroutine);
                _ringCoroutine = null;
            }

            if (_ring != null)
                _ring.enabled = false;
        }

        private void OnDisable()
        {
            StopRing();
            StopReviveAppear();
            StopColorCoroutine();
        }

        private void OnDestroy()
        {
            if (_ringMaterial != null)
                Destroy(_ringMaterial);
        }

        private IEnumerator ReviveAppearRoutine()
        {
            const float duration = 0.45f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                Color color = _originalColor;
                color.a = Mathf.Lerp(0f, _invincibleAlpha, Mathf.Clamp01(elapsed / duration));
                ApplyColor(color);
                yield return null;
            }

            _reviveAppearCoroutine = null;
        }

        private void StopReviveAppear()
        {
            if (_reviveAppearCoroutine == null)
                return;

            StopCoroutine(_reviveAppearCoroutine);
            _reviveAppearCoroutine = null;
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
