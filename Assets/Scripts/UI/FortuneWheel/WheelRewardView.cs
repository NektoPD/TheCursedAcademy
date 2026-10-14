using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FortuneWheel
{
    public class WheelRewardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField, Range(0.1f, 1f)] private float _appearStartScale = 0.8f;

        private CanvasGroup _canvasGroup;
        private Tween _appearanceTween;
        private Vector3 _initialScale;
        private float _initialAlpha;
        private bool _initialInteractable;
        private bool _initialBlocksRaycasts;
        private bool _appearanceCached;

        public void Initialize(string title, string description, Sprite icon)
        {
            _title.text = title;
            _description.text = description;
            _icon.sprite = icon;
            _icon.enabled = icon != null;
        }

        public void PrepareAppearance()
        {
            if (!_appearanceCached)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();

                _initialScale = transform.localScale;
                _initialAlpha = _canvasGroup.alpha;
                _initialInteractable = _canvasGroup.interactable;
                _initialBlocksRaycasts = _canvasGroup.blocksRaycasts;
                _appearanceCached = true;
            }

            StopAppearance();
            transform.localScale = _initialScale * _appearStartScale;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public Tween PlayAppearance(float duration)
        {
            _appearanceTween = DOTween.Sequence()
                .Join(_canvasGroup.DOFade(_initialAlpha, Mathf.Max(0.01f, duration))
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOScale(_initialScale, Mathf.Max(0.01f, duration))
                    .SetEase(Ease.OutBack))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _appearanceTween = null;
                    _canvasGroup.interactable = _initialInteractable;
                    _canvasGroup.blocksRaycasts = _initialBlocksRaycasts;
                });
            return _appearanceTween;
        }

        public void StopAppearance()
        {
            _appearanceTween?.Kill();
            _appearanceTween = null;
            if (!_appearanceCached)
                return;

            transform.localScale = _initialScale;
            _canvasGroup.alpha = _initialAlpha;
            _canvasGroup.interactable = _initialInteractable;
            _canvasGroup.blocksRaycasts = _initialBlocksRaycasts;
        }

        private void OnDisable()
        {
            StopAppearance();
        }
    }
}
