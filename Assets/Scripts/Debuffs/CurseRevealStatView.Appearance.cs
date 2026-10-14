using DG.Tweening;
using UnityEngine;

namespace Debuffs
{
    public partial class CurseRevealStatView
    {
        private CanvasGroup _appearanceCanvasGroup;
        private Tween _appearanceTween;
        private float _originalAppearanceAlpha;
        private bool _originalAppearanceInteractable;
        private bool _originalAppearanceBlocksRaycasts;
        private bool _hasAppearanceSnapshot;
        private bool _appearancePrepared;

        public void PrepareAppearance()
        {
            StopAppearance();

            // Resolve lazily: the overlay can prepare an inactive card before Awake.
            if (_appearanceCanvasGroup == null)
            {
                _appearanceCanvasGroup = GetComponent<CanvasGroup>();
                if (_appearanceCanvasGroup == null)
                    _appearanceCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _originalAppearanceAlpha = _appearanceCanvasGroup.alpha;
            _originalAppearanceInteractable = _appearanceCanvasGroup.interactable;
            _originalAppearanceBlocksRaycasts = _appearanceCanvasGroup.blocksRaycasts;
            _hasAppearanceSnapshot = true;
            _appearancePrepared = true;

            _appearanceCanvasGroup.alpha = 0f;
            _appearanceCanvasGroup.interactable = false;
            _appearanceCanvasGroup.blocksRaycasts = false;
        }

        public void PlayAppearance(float delay, float duration)
        {
            if (!_appearancePrepared)
                return;

            _appearancePrepared = false;
            if (!isActiveAndEnabled || _appearanceCanvasGroup == null)
            {
                StopAppearance();
                return;
            }

            // The tween's independent update also applies to its start delay.
            _appearanceTween = _appearanceCanvasGroup
                .DOFade(_originalAppearanceAlpha, Mathf.Max(0.01f, duration))
                .SetDelay(Mathf.Max(0f, delay))
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(CompleteAppearance);
        }

        public void StopAppearance()
        {
            _appearancePrepared = false;

            // Kill without completion so cancellation cannot run completion callbacks.
            if (_appearanceTween != null)
            {
                _appearanceTween.Kill(false);
                _appearanceTween = null;
            }

            RestoreAppearance();
        }

        private void CompleteAppearance()
        {
            _appearanceTween = null;
            RestoreAppearance();
        }

        private void RestoreAppearance()
        {
            if (!_hasAppearanceSnapshot)
                return;

            if (_appearanceCanvasGroup != null)
            {
                _appearanceCanvasGroup.alpha = _originalAppearanceAlpha;
                _appearanceCanvasGroup.interactable = _originalAppearanceInteractable;
                _appearanceCanvasGroup.blocksRaycasts = _originalAppearanceBlocksRaycasts;
            }

            _hasAppearanceSnapshot = false;
        }

        private void OnDisable()
        {
            StopAppearance();
        }

        private void OnDestroy()
        {
            StopAppearance();
        }
    }
}
