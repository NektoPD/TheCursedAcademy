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
            public Vector3 InitialScale;
            public Tween Tween;
        }

        private void Update()
        {
            foreach (KeyValuePair<Image, IconMotion> entry in _iconMotions)
            {
                if (entry.Value.Tween != null
                    && (entry.Key == null || !entry.Key.gameObject.activeInHierarchy))
                    StopIconAnimation(entry.Key, entry.Value);
            }
        }

        private static void StopIconAnimation(Image icon, IconMotion motion)
        {
            motion.Tween?.Kill(false);
            motion.Tween = null;

            if (icon != null)
                icon.transform.localScale = motion.InitialScale;
        }

        private IconMotion PrepareIconAnimation(Image icon)
        {
            if (!_iconMotions.TryGetValue(icon, out IconMotion motion))
            {
                motion = new IconMotion { InitialScale = icon.transform.localScale };
                _iconMotions.Add(icon, motion);
            }

            StopIconAnimation(icon, motion);
            return motion;
        }

        private void RevealAnimatedIcon(Image icon)
        {
            if (icon == null || icon.sprite == null || icon.gameObject.activeSelf)
                return;

            IconMotion motion = PrepareIconAnimation(icon);
            Transform iconTransform = icon.transform;
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
                StopIconAnimation(entry.Key, entry.Value);
            }

            if (_statsView != null)
                _statsView.StopAppearance();
        }
    }
}
