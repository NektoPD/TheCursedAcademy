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

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            var suquence = DOTween.Sequence();

            suquence.Join(_rectTransform.DOScale(_endScale, _duration).SetEase(Ease.InOutQuad))
                .Join(_rectTransform.DORotate(_rotation, _duration).SetEase(Ease.InOutQuad))
                .SetLoops(-1, LoopType.Yoyo);

            suquence.SetUpdate(true).Play();
        }
    }
}