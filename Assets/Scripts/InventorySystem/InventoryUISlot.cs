using Items.BaseClass;
using Items.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryUISlot : MonoBehaviour
    {
        [SerializeField] private Image _slotImageHolder;
        [SerializeField] private Image _reloadProgressImage;
        [SerializeField] private TMP_Text _levelText;

        private Item _item;
        private int _displayedLevel = -1;
        
        public ItemVariations ItemVariation { get; private set; }
        public bool IsActive { get; private set; }

        private void Awake()
        {
            if (_reloadProgressImage == null)
                _reloadProgressImage = CreateReloadProgressImage();

            HideReloadProgress();
            RefreshLevel();
        }

        private void Update()
        {
            bool isReloading = _item != null && _item.IsReloading;
            _reloadProgressImage.enabled = isReloading;

            if (isReloading)
                _reloadProgressImage.fillAmount = _item.ReloadProgress;

            RefreshLevel();
        }

        public void Enable()
        {
            IsActive = true;
            gameObject.SetActive(IsActive);
        }

        public void Disable()
        {
            IsActive = false;
            _item = null;
            HideReloadProgress();
            RefreshLevel();
            gameObject.SetActive(IsActive);
        }

        public void SetItem(Item item)
        {
            _item = item;
            _slotImageHolder.enabled = true;
            _slotImageHolder.sprite = item.Data.ItemIcon;
            _reloadProgressImage.sprite = item.Data.ItemIcon;
            ItemVariation = item.VisualData.Variation;
            RefreshLevel();
        }

        public void ResetSlot()
        {
            _item = null;
            _slotImageHolder.enabled = false;
            HideReloadProgress();
            RefreshLevel();
        }

        private void RefreshLevel()
        {
            int level = _item != null ? _item.CurrentLevel : 0;
            if (_levelText == null || _displayedLevel == level)
                return;

            _displayedLevel = level;
            _levelText.enabled = level > 0;
            _levelText.text = level > 0 ? level.ToString() : string.Empty;
        }

        private Image CreateReloadProgressImage()
        {
            var progressObject = new GameObject("Reload Progress", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var progressTransform = progressObject.GetComponent<RectTransform>();
            progressTransform.SetParent(_slotImageHolder.rectTransform, false);
            progressTransform.anchorMin = Vector2.zero;
            progressTransform.anchorMax = Vector2.one;
            progressTransform.offsetMin = Vector2.zero;
            progressTransform.offsetMax = Vector2.zero;

            var progressImage = progressObject.GetComponent<Image>();
            progressImage.color = new Color(0f, 0f, 0f, 0.6f);
            progressImage.raycastTarget = false;
            progressImage.type = Image.Type.Filled;
            progressImage.fillMethod = Image.FillMethod.Radial360;
            progressImage.fillOrigin = (int)Image.Origin360.Bottom;
            progressImage.fillClockwise = true;
            progressImage.fillAmount = 0f;
            return progressImage;
        }

        private void HideReloadProgress()
        {
            if (_reloadProgressImage == null)
                return;

            _reloadProgressImage.fillAmount = 0f;
            _reloadProgressImage.enabled = false;
        }
    }
}
