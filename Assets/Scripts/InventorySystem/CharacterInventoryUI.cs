using System;
using System.Linq;
using Items;
using Items.BaseClass;
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
        }

        private void Start()
        {
            DisableAllSlots();
        }

        private void OnDisable()
        {
            if (_inventory != null)
                _inventory.ItemAdded -= EnableItemSlot;
        }

        private void EnableItemSlot(Item item)
        {

            InventoryUISlot slotToEnable = _uiSlots.FirstOrDefault(slot => !slot.IsActive);

            //if (slotToEnable == null) return;
            slotToEnable.Enable();
            slotToEnable.SetItemSprite(item.Data.ItemIcon);
        }

        private void DisableAllSlots()
        {
            foreach (InventoryUISlot inventoryUISlot in _uiSlots)
            {
                inventoryUISlot.Disable();
            }
        }
    }
}