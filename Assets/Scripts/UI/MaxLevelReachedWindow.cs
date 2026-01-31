using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MaxLevelReachedWindow : Window
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _lockedChest;
        [SerializeField] private Sprite _openChest;

        public override void OpenWindow()
        {
            base.OpenWindow();

            _image.sprite = _lockedChest;
            _image.transform.localScale = Vector3.one;

            var seq = DOTween.Sequence()
                .SetUpdate(true);

            seq.Append(_image.transform
                    .DOScale(1.2f, 0.5f)
                    .SetLoops(2, LoopType.Yoyo))
                .AppendCallback(() => _image.sprite = _openChest)
                .Append(_image.transform
                    .DOScale(1.3f, 1f)
                    .SetLoops(2, LoopType.Yoyo))
                .AppendCallback(CloseWindow);
        }

    }
}