using Data;
using TMPro;
using UI.Applicators.ClickHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(ItemClickHandler))]
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private Image _image;

        private ItemClickHandler _clickHandler;

        private void Awake()
        {
            _clickHandler = GetComponent<ItemClickHandler>();
        }

        public void Initialize(ItemVisualData visualData)
        {
            _textName.text = visualData.Name;
            _textDescription.text = visualData.Description;
            _image.sprite = visualData.Sprite;
            _clickHandler.SetData(visualData);
        }
    }
}