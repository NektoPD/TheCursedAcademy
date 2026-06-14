using UI.Applicators.ClickHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators
{
    public class TextureApplicator : MonoBehaviour
    {
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;
        [SerializeField] private TextureClickHandler[] _items;
        [SerializeField] private Image _defaultItem;

        private Image _currentItem;

        private void Awake()
        {
            _currentItem = _defaultItem;
        }

        private void OnEnable()
        {
            Applicate(_defaultItem);

            foreach (var perk in _items)
                perk.Clicked += Applicate;
        }

        private void OnDisable()
        {
            foreach (var perk in _items)
                perk.Clicked -= Applicate;
        }

        private void Applicate(Image image)
        {
            _currentItem.sprite = _offSprite;
            image.sprite = _onSprite;
            _currentItem = image;
        }
    }
}