using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FortuneWheel
{
    public class WheelSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Image _new;
        [SerializeField] private RectTransform _pulseTarget;
        [SerializeField] private float _pulseScale = 1.2f;
        [SerializeField] private float _pulseDuration = 0.5f;

        private Tween _pulseTween;

        public void Set(WheelReward reward, bool isNew = false)
        {
            if (reward == null)
                return;

            if (_icon != null)
            {
                Sprite sprite = reward.Sprite;
                _icon.sprite = sprite;
                _icon.enabled = sprite != null;
            }

            if (_label != null)
                _label.text = reward.Label;

            if (_new != null)
                _new.gameObject.SetActive(isNew);
        }

        public void PlayPulse()
        {
            RectTransform target = _pulseTarget != null ? _pulseTarget : transform as RectTransform;
            if (target == null)
                return;

            StopPulse();
            target.localScale = Vector3.one;
            _pulseTween = target
                .DOScale(_pulseScale, _pulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void StopPulse()
        {
            _pulseTween?.Kill();
            _pulseTween = null;

            RectTransform target = _pulseTarget != null ? _pulseTarget : transform as RectTransform;
            if (target != null)
                target.localScale = Vector3.one;
        }

        private void OnDisable() => StopPulse();
    }
}
