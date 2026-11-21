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

        private void HideReloadProgress()
        {
            if (_reloadProgressImage == null)
                return;

            _reloadProgressImage.fillAmount = 0f;
            _reloadProgressImage.enabled = false;
        }
    }
}
