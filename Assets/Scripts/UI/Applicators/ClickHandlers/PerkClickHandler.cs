using Data;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators.ClickHandlers
{
    public class PerkClickHandler : BaseClickHandler<PerkVisualData>
    {
        [SerializeField] private Image[] _levels;
        [SerializeField] private Sprite _on;
        [SerializeField] private PerkApplicator _applicator;
        [SerializeField] private Image _maxImage;

        private Queue<Image> _currentLevels;
        private Image _background;
        private Color _backgroundColor;
        private Tween _selectionTween;
        private Tween _cardTween;
        private Tween _levelTween;
        private Image _lastBoughtLevel;
        private Vector3 _lastLevelScale;
        private Vector3 _cardScale;

        private void Awake()
        {
            _cardScale = transform.localScale;
            _background = GetComponent<Image>();
            if (_background != null)
                _backgroundColor = _background.color;
        }

        private void Start()
        {
            _currentLevels = new Queue<Image>();

            foreach (var level in _levels)
                _currentLevels.Enqueue(level);

            for (int i = 0; i < _applicator.PerkController.GetPerkLevel(Data.Type); i++)
            {
                Image level = _currentLevels.Dequeue();
                level.sprite = _on;
            }

            _maxImage.enabled = _currentLevels.Count == 0;
        }

        private void OnEnable()
        {
            _applicator.Buyed += Up;
            _applicator.Selected += OnSelected;
        }

        private void OnDisable()
        {
            _applicator.Buyed -= Up;
            _applicator.Selected -= OnSelected;
            _selectionTween?.Kill();
            _cardTween?.Kill();
            _levelTween?.Kill();
            transform.localScale = _cardScale;
            if (_background != null)
                _background.color = _backgroundColor;
            if (_lastBoughtLevel != null)
                _lastBoughtLevel.transform.localScale = _lastLevelScale;
        }

        private void OnSelected(PerkVisualData data)
        {
            if (_background == null)
                return;

            _cardTween?.Kill();
            transform.localScale = _cardScale;
            if (data == Data)
            {
                _cardTween = transform.DOScale(_cardScale * 1.08f, 0.16f)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetUpdate(true);
            }

            _selectionTween?.Kill();
            Color selectedColor = Color.Lerp(_backgroundColor,
                new Color(1f, 0.85f, 0.48f, _backgroundColor.a), 0.55f);
            _selectionTween = _background.DOColor(data == Data ? selectedColor : _backgroundColor, 0.2f)
                .SetUpdate(true);
        }

        public void Up(PerkVisualData data)
        {
            if (data != Data)
                return;

            if (_currentLevels.Count == 0)
                return;

            Image level = _currentLevels.Dequeue();
            level.sprite = _on;
            _levelTween?.Kill();
            if (_lastBoughtLevel != null)
                _lastBoughtLevel.transform.localScale = _lastLevelScale;
            _lastBoughtLevel = level;
            _lastLevelScale = level.transform.localScale;
            _levelTween = level.transform.DOScale(_lastLevelScale * 1.35f, 0.2f)
                .SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
        }
    }
}