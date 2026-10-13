using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    [RequireComponent(typeof(Toggle))]
    public sealed class CurseRevealToggleView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Image _panel;
        [SerializeField] private Image _track;
        [SerializeField] private Image _handle;
        [SerializeField, Min(0.01f)] private float _duration = 0.2f;
        [SerializeField] private Color _panelOff = new(0.12f, 0.16f, 0.23f, 1f);
        [SerializeField] private Color _panelOn = new(0.25f, 0.15f, 0.36f, 1f);
        [SerializeField] private Color _trackOff = new(0.24f, 0.29f, 0.37f, 1f);
        [SerializeField] private Color _trackOn = new(0.64f, 0.35f, 0.85f, 1f);
        [SerializeField] private Color _handleOff = new(0.7f, 0.76f, 0.84f, 1f);
        [SerializeField] private Color _handleOn = new(1f, 0.9f, 0.69f, 1f);

        private Tween _tween;
        private float _progress;

        private void Awake()
        {
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
            ApplyState(_toggle.isOn ? 1f : 0f);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
            StopAnimation();
            ApplyState(_toggle.isOn ? 1f : 0f);
        }

        private void OnValueChanged(bool enabled)
        {
            StopAnimation();
            _tween = DOTween.To(() => _progress, ApplyState, enabled ? 1f : 0f,
                    Mathf.Max(0.01f, _duration))
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        private void StopAnimation()
        {
            _tween?.Kill(false);
            _tween = null;
        }

        private void ApplyState(float progress)
        {
            _progress = progress;

            if (_panel != null)
                _panel.color = Color.Lerp(_panelOff, _panelOn, progress);

            if (_track != null)
                _track.color = Color.Lerp(_trackOff, _trackOn, progress);

            if (_handle == null)
                return;

            _handle.color = Color.Lerp(_handleOff, _handleOn, progress);
            RectTransform handleTransform = _handle.rectTransform;
            float left = Mathf.Lerp(0.08f, 0.52f, progress);
            handleTransform.anchorMin = new Vector2(left, 0.14f);
            handleTransform.anchorMax = new Vector2(left + 0.4f, 0.86f);
        }
    }
}
