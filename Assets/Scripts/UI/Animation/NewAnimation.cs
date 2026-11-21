using UnityEngine;
using DG.Tweening;

namespace UI.Animation
{
    [RequireComponent(typeof(RectTransform))]
    public class NewAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 _endScale;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private float _duration;
        [SerializeField] private bool _useLocalRotation;

        private RectTransform _rectTransform;
        private Vector3 _initialScale;
        private Quaternion _initialRotation;
        private Sequence _sequence;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialScale = _rectTransform.localScale;
            _initialRotation = _rectTransform.localRotation;
        }

        private void OnEnable()
        {
            Tween rotationTween = _useLocalRotation
                ? _rectTransform.DOLocalRotate(_rotation, _duration)
                : _rectTransform.DORotate(_rotation, _duration);

            _sequence = DOTween.Sequence()
                .Join(_rectTransform.DOScale(_endScale, _duration).SetEase(Ease.InOutQuad))
                .Join(rotationTween.SetEase(Ease.InOutQuad))
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
            _sequence.Play();
        }

        private void OnDisable()
        {
            _sequence?.Kill();
            _sequence = null;
            _rectTransform.localScale = _initialScale;
            _rectTransform.localRotation = _initialRotation;
        }
    }
}