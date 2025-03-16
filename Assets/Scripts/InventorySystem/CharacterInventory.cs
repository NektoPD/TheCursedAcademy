using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Items.BaseClass;
using UnityEngine;

namespace InventorySystem
{
    public class CharacterInventory
    {
        private List<Item> _collectedItems = new();

        public event Action<Item> ItemAdded;
        public event Action<Item> ItemRemoved;

        public IReadOnlyCollection<Item> Items => _collectedItems;

        public void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_collectedItems.Contains(item))
                return;

            _collectedItems.Add(item);
            ItemAdded?.Invoke(item);
        }

        public void RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_collectedItems.Contains(item))
                _collectedItems.Remove(item);

            ItemRemoved?.Invoke(item);
        }
    }
}