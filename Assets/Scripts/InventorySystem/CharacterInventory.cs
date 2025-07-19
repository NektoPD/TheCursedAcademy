using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Items.BaseClass;
using Items.Enums;
using StatistiscSystem;
using UnityEngine;

namespace InventorySystem
{
    public class CharacterInventory
    {
        private List<Item> _collectedItems = new();
        private Dictionary<ItemVariations, DateTime> _itemAddTimes = new();

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
            
            if (!_itemAddTimes.ContainsKey(item.Data.ItemVariation))
            {
                _itemAddTimes[item.Data.ItemVariation] = DateTime.Now;
            }

            ItemAdded?.Invoke(item);
        }

        public void RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_collectedItems.Contains(item))
                _collectedItems.Remove(item);
            
            if (_collectedItems.All(i => i.Data.ItemVariation != item.Data.ItemVariation))
            {
                _itemAddTimes.Remove(item.Data.ItemVariation);
            }

            ItemRemoved?.Invoke(item);
        }

        public List<ItemStatistics> GetItemStatisticsList()
        {
            if (_collectedItems.Count <= 0)
            {
                Debug.Log("no items");
                return null;
            }
            
            List<ItemStatistics> itemStatisticsList = new();
            DateTime currentTime = DateTime.Now;
            
            foreach (Item collectedItem in _collectedItems)
            {
                TimeSpan timeInInventory = TimeSpan.Zero;
                if (_itemAddTimes.TryGetValue(collectedItem.Data.ItemVariation, out DateTime addTime))
                {
                    timeInInventory = currentTime - addTime;
                }
                
                float dps = collectedItem.Data.Damage / collectedItem.Data.Cooldown;
                
                var statisticsData = new ItemStatistics(
                    collectedItem.Data, 
                    collectedItem.Data.Damage, 
                    collectedItem.CurrentLevel, 
                    dps, 
                    timeInInventory
                );
                
                itemStatisticsList.Add(statisticsData);
            }
            
            return itemStatisticsList;
        }
    }
}