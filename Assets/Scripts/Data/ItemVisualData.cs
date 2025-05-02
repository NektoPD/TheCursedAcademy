using Items.ItemData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Data
{
    public class ItemVisualData : MonoBehaviour, IVisualData
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private Image _image;

        private string _name;
        private string _description;
        private Sprite _sprite;
        private ItemDataConfig _config;

        public string Name => _name;

        public string Description => _description;

        public Sprite Sprite => _sprite;

        public ItemDataConfig Config => _config;

        public void Initialize(ItemDataConfig itemData)
        {
            _config = itemData;
            _name = Translator.Translate(itemData.ItemNameRu, itemData.ItemNameEn, itemData.ItemNameTr);
            _description = Translator.Translate(itemData.DescriptionRu, itemData.DescriptionEn, itemData.DescriptionTr);
            _sprite = itemData.ItemIcon;

            _textName.text = _name;
            _textDescription.text = _description;
            _image.sprite = _sprite;
        }
    }
}