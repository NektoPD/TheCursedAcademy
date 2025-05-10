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
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private Image _image;
        [SerializeField] private Image _new;

        private ItemClickHandler _clickHandler;

        private void Awake()
        {
            _clickHandler = GetComponent<ItemClickHandler>();
        }

        public void Initialize(ItemVisualData visualData, bool isNew, int level)
        {
            _textName.text = visualData.Name;
            _textDescription.text = visualData.Description;
            _image.sprite = visualData.Sprite;
            _textLevel.text = $"{level + 1}";

             _new.gameObject.SetActive(isNew);

            _clickHandler.SetData(visualData);
        }
    }
}