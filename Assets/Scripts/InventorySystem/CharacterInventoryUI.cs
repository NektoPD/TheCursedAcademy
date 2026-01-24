using System;
using System.Linq;
using Items;
using Items.BaseClass;
using TheraBytes.BetterUi;
using UnityEngine;

namespace InventorySystem
{
    public class CharacterInventoryUI : MonoBehaviour
    {
        [SerializeField] private InventoryUISlot[] _uiSlots;

        private CharacterInventory _inventory;

        public void Initialize(CharacterInventory inventory)
        {
            _inventory = inventory;

            _inventory.ItemAdded += EnableItemSlot;
            _inventory.ItemRemoved += DisableItemSlot;
        }


        private void OnDisable()
        {
            if (_inventory != null)
            {
                _inventory.ItemAdded -= EnableItemSlot;
                _inventory.ItemRemoved -= DisableItemSlot;
            }
        }

        public void DisableAllSlots()
        {
            foreach (InventoryUISlot inventoryUISlot in _uiSlots)
            {
                inventoryUISlot.Disable();
            }
        }

        private void EnableItemSlot(Item item)
        {
            InventoryUISlot slotToEnable = _uiSlots.FirstOrDefault(slot => !slot.IsActive);

            slotToEnable.Enable();
            slotToEnable.SetItem(item.Data.ItemIcon, item.VisualData.Variation);
        }

        private void DisableItemSlot(Item item)
        {
            InventoryUISlot slotToEnable = _uiSlots.FirstOrDefault(slot => slot.ItemVariation == item.VisualData.Variation);

            if (slotToEnable == null)
            {
                Debug.LogError("slot is null");
                return;
            }
            
            slotToEnable.Disable();
        }
    }
}