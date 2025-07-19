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
            if (_colorChangeCoroutine != null)
            {
                StopCoroutine(_colorChangeCoroutine);
                _colorChangeCoroutine = null;
            }

            _isPulsating = false;
            ApplyColor(_originalColor);
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

        private void ApplyColor(Color color)
        {
            _spriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_Color", color);
            _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}