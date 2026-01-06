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
        private readonly List<Item> _collectedItems = new();

        private readonly Dictionary<ItemVariations, float> _itemAddTimesSec = new();

        private readonly Dictionary<ItemVariations, float> _totalDamageByVariation = new();

        public event Action<Item> ItemAdded;
        public event Action<Item> ItemRemoved;

        public IReadOnlyCollection<Item> Items => _collectedItems;
        
        public void AddItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_collectedItems.Contains(item)) return;

            _collectedItems.Add(item);

            var variation = item.Data.ItemVariation;

            if (!_itemAddTimesSec.ContainsKey(variation))
                _itemAddTimesSec[variation] = Time.timeSinceLevelLoad;

            if (!_totalDamageByVariation.ContainsKey(variation))
                _totalDamageByVariation[variation] = 0f;

            item.DamageDealt += OnItemDamageDealt;
            
            ItemAdded?.Invoke(item);
        }

        public void RemoveItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (_collectedItems.Contains(item))
                _collectedItems.Remove(item);

            var variation = item.Data.ItemVariation;

            if (_collectedItems.All(i => i.Data.ItemVariation != variation))
                _itemAddTimesSec.Remove(variation);

            item.DamageDealt -= OnItemDamageDealt;
            
            ItemRemoved?.Invoke(item);
        }

        public void RegisterDamage(ItemVariations variation, float damage)
        {
            if (damage <= 0f) return;

            if (_totalDamageByVariation.ContainsKey(variation))
                _totalDamageByVariation[variation] += damage;
            else
                _totalDamageByVariation[variation] = damage;
        }

        public List<ItemStatistics> GetItemStatisticsList()
        {
            if (_collectedItems.Count <= 0)
            {
                return null;
            }

            var currentTime = Time.timeSinceLevelLoad;

            var byVariation = _collectedItems
                .GroupBy(i => i.Data.ItemVariation)
                .ToList();

            List<ItemStatistics> itemStatisticsList = new();

            foreach (var group in byVariation)
            {
                var item = group.First();
                var variation = group.Key;

                float addSec = _itemAddTimesSec.TryGetValue(variation, out var t) ? t : currentTime;
                float timeInInvSec = Mathf.Max(0f, currentTime - addSec);

                var timeInInventory = TimeSpan.FromSeconds(timeInInvSec);

                float totalDamage = _totalDamageByVariation.TryGetValue(variation, out var dmg) ? dmg : 0f;

                float dps = 0f;
                if (item.Data.Cooldown > 0f)
                    dps = item.Data.Damage / item.Data.Cooldown;

                var statisticsData = new ItemStatistics(
                    item.Data,
                    totalDamage,
                    item.CurrentLevel,
                    dps,
                    timeInInventory
                );

                itemStatisticsList.Add(statisticsData);
            }

            return itemStatisticsList;
        }
        
        private void OnItemDamageDealt(ItemVariations variation, float damage)
        {
            if (damage <= 0f) return;

            if (_totalDamageByVariation.ContainsKey(variation))
                _totalDamageByVariation[variation] += damage;
            else
                _totalDamageByVariation[variation] = damage;
        }
    }
}
