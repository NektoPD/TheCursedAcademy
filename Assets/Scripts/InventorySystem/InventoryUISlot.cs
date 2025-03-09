using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryUISlot : MonoBehaviour
    {
        [SerializeField] private Image _slotImageHolder;

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

        public void SetItemSprite(Sprite sprite)
        {
            _slotImageHolder.enabled = true;
            _slotImageHolder.sprite = sprite;
        }

        public void ResetSlot()
        {
            _slotImageHolder.enabled = false;
        }
    }
}