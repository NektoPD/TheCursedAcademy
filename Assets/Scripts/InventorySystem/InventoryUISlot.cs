using Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryUISlot : MonoBehaviour
    {
        [SerializeField] private Image _slotImageHolder;
        
        public ItemVariations ItemVariation { get; private set; }
        public bool IsActive { get; private set; }

        public void Enable()
        {
            IsActive = true;
            gameObject.SetActive(IsActive);
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);
        }

        public void SetItem(Sprite sprite, ItemVariations itemVariations)
        {
            _slotImageHolder.enabled = true;
            _slotImageHolder.sprite = sprite;
            ItemVariation = itemVariations;
        }

        public void ResetSlot()
        {
            _slotImageHolder.enabled = false;
        }
    }
}