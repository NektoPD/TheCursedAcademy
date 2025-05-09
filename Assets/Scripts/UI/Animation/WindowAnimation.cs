using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _duration;
        [SerializeField] private Image _background;
        [SerializeField] [Range(0, 1)] private float _alfa = 0.2f;

        private RectTransform _rectTransform;
        private float _startY;
        private Tween _currentTween;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startY = _rectTransform.anchoredPosition.y;
        }

        public void Open()
        {
            if (_currentTween != null && _currentTween.IsActive())
                _currentTween.Kill();

            gameObject.SetActive(true);

            _currentTween = DoFadeBackground(0, 0);
            _currentTween =_rectTransform.DOAnchorPos(_targetPosition, _duration).OnComplete(() => DoFadeBackground(_alfa, _duration / 2)).SetUpdate(true);
        }

        public void Close()
        {
            if (_currentTween != null && _currentTween.IsActive())
                _currentTween.Kill();

            var newPosition = new Vector2(_rectTransform.anchoredPosition.x, _startY);

            var suquence = DOTween.Sequence();

            suquence.Append(DoFadeBackground(0, _duration / 2))
                .Append(_rectTransform.DOAnchorPos(newPosition, _duration))
                .SetUpdate(true);

            _currentTween =suquence.OnComplete(() => gameObject.SetActive(false)).Play();
        }

        public void StopTime() => Time.timeScale = 0f;

        public void StartTime() => Time.timeScale = 1f;

        private Tween DoFadeBackground(float alfa, float duration)
        {
            if (_background == null)
                return null;
            
            return _background.DOFade(alfa, duration).SetUpdate(true);
        }
    }
}