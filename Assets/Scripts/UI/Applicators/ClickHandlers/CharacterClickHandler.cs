using Data;
using DG.Tweening;
using UnityEngine;

namespace UI.Applicators.ClickHandlers
{
    public class CharacterClickHandler : BaseClickHandler<CharacterVisualData>
    {
        private CharacterApplicator _applicator;
        private Tween _cardTween;
        private Vector3 _cardScale;

        private void Awake()
        {
            _applicator = GetComponentInParent<CharacterApplicator>(true);
            _cardScale = transform.localScale;
        }

        private void OnEnable()
        {
            _applicator.Selected += OnSelected;
        }

        private void OnDisable()
        {
            _applicator.Selected -= OnSelected;
            _cardTween?.Kill();
            transform.localScale = _cardScale;
        }

        private void OnSelected(CharacterVisualData data)
        {
            _cardTween?.Kill();
            transform.localScale = _cardScale;
            if (data != Data)
                return;

            _cardTween = transform.DOScale(_cardScale * 1.08f, 0.16f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);
        }
    }
}
