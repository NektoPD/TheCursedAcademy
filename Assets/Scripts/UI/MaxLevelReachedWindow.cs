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
            ShowMaxLevelAnimation();
        }
        
        public void ShowMaxLevelAnimation()
        {
            gameObject.SetActive(true);

            _image.sprite = _lockedChest;
            _image.transform.localScale = Vector3.one;

            // Пульсируем
            var seq = DOTween.Sequence();

            seq.Append(_image.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo)) // первый пульс 1 сек
                .AppendCallback(() =>
                {
                    _image.sprite = _openChest; // меняем спрайт
                })
                .Append(_image.transform.DOScale(1.3f, 1f).SetLoops(2, LoopType.Yoyo)) // второй пульс 2 сек
                .AppendCallback(() =>
                {
                    CloseWindow(); // закрываем окно
                });
        }

    }
}