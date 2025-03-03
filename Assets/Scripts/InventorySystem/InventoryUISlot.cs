using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryUISlot : MonoBehaviour
    {
        [SerializeField] private Image _slotImageHolder;

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