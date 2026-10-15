using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public partial class CurseRevealOverlay
    {
        [SerializeField, Min(0.01f)] private float _iconAppearDuration = 0.25f;
        [SerializeField, Range(0.1f, 1f)] private float _iconAppearStartScale = 0.85f;
        [SerializeField, Min(0.01f)] private float _iconPulseDuration = 0.7f;
        [SerializeField, Range(1f, 1.15f)] private float _iconPulseScale = 1.04f;

        private readonly Dictionary<Image, IconMotion> _iconMotions = new();

        private sealed class IconMotion
        {
            public Transform Target;
            public Vector3 InitialScale;
            public Tween Tween;
        }

        private void Update()
        {
            foreach (KeyValuePair<Image, IconMotion> entry in _iconMotions)
            {
                if (entry.Value.Tween != null
                    && (entry.Key == null || !entry.Key.gameObject.activeInHierarchy))
                    StopIconAnimation(entry.Value);
            }
        }

        private static void StopIconAnimation(IconMotion motion)
        {
            motion.Tween?.Kill(false);
            motion.Tween = null;

            if (motion.Target != null)
                motion.Target.localScale = motion.InitialScale;
        }

        private IconMotion PrepareIconAnimation(Image icon)
        {
            if (!_iconMotions.TryGetValue(icon, out IconMotion motion))
            {
                Transform target = icon.transform;
                Transform parent = target.parent;
                if (parent != null && parent.TryGetComponent<Image>(out _))
                    target = parent;

                motion = new IconMotion
                {
                    Target = target,
                    InitialScale = target.localScale
                };
                _iconMotions.Add(icon, motion);
            }

            StopIconAnimation(motion);
            return motion;
        }

        private void RevealAnimatedIcon(Image icon)
        {
            if (icon == null || icon.sprite == null || icon.gameObject.activeSelf)
                return;

            IconMotion motion = PrepareIconAnimation(icon);
            // Animate the frame as a unit with its child icon, without doubling the scale.
            Transform iconTransform = motion.Target;
            iconTransform.localScale = motion.InitialScale * _iconAppearStartScale;
            icon.gameObject.SetActive(true);
            motion.Tween = iconTransform
                .DOScale(motion.InitialScale, Mathf.Max(0.01f, _iconAppearDuration))
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (_choiceMade || !isActiveAndEnabled || icon == null
                        || !icon.gameObject.activeInHierarchy)
                        return;

                    motion.Tween = iconTransform
                        .DOScale(motion.InitialScale * _iconPulseScale, Mathf.Max(0.01f, _iconPulseDuration))
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetUpdate(true);
                });
        }

        private void StopPresentationAnimations()
        {
            foreach (KeyValuePair<Image, IconMotion> entry in _iconMotions)
            {
                StopIconAnimation(entry.Value);
            }

            if (_statsView != null)
                _statsView.StopAppearance();
        }
    }
}
