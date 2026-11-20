using System;
using System.Collections.Generic;
using System.Linq;
using Items.BaseClass;
using Items.Enums;
using Items.ItemData;
using StatistiscSystem;
using UnityEngine;

namespace InventorySystem
{
    public class CharacterInventory
    {
        private readonly List<Item> _collectedItems = new();

        private readonly Dictionary<ItemVariations, ItemHistory> _itemHistory = new();
        private readonly HashSet<Item> _trackedItems = new();

        public int InventoryLimit { get; private set; }

        public ItemVariations StartVariation { get; private set; }

        public CharacterInventory(int inventoryLimit)
        {
            InventoryLimit = inventoryLimit;
        }

        public void SetStartVariation(ItemVariations startVariation)
        {
            StartVariation = startVariation;
        }

        public event Action<Item> ItemAdded;
        public event Action<Item> ItemRemoved;
        public event Action MaxLevelReached;

        public IReadOnlyList<Item> Items => _collectedItems;

        public void AddItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_collectedItems.Contains(item)) return;

            _collectedItems.Add(item);

            var variation = item.Data.ItemVariation;

            if (!_itemHistory.TryGetValue(variation, out var history))
            {
                history = new ItemHistory(item.Data);
                _itemHistory.Add(variation, history);
            }

            history.EquippedSince ??= Time.timeSinceLevelLoad;
            history.HighestLevel = Mathf.Max(history.HighestLevel, item.CurrentLevel);

            if (_trackedItems.Add(item))
                item.DamageDealt += OnItemDamageDealt;

            item.MaxLevelReached += OnItemMaxLevelReached;

            ItemAdded?.Invoke(item);
        }

        private void OnItemMaxLevelReached()
        {
            MaxLevelReached?.Invoke();
        }

        public void RemoveItem(ItemVariations itemVariations)
        {
            Item itemToRemove = _collectedItems.FirstOrDefault(item => item.VisualData.Variation == itemVariations);

            if (itemToRemove == null)
            {
                return;
            }

            if (itemToRemove.VisualData.Variation == StartVariation)
            {
                return;
            }

            _collectedItems.Remove(itemToRemove);

            var variation = itemToRemove.Data.ItemVariation;

            var history = _itemHistory[variation];
            history.HighestLevel = Mathf.Max(history.HighestLevel, itemToRemove.CurrentLevel);

            if (_collectedItems.All(i => i.Data.ItemVariation != variation))
            {
                history.SecondsInInventory = history.GetSecondsInInventory(Time.timeSinceLevelLoad);
                history.EquippedSince = null;
            }

            itemToRemove.MaxLevelReached -= OnItemMaxLevelReached;

            ItemRemoved?.Invoke(itemToRemove);
        }

        public bool CanFitOneMoreItem()
        {
            return _collectedItems.Count + 1 <= InventoryLimit;
        }

        public List<ItemStatistics> GetItemStatisticsList()
        {
            foreach (var item in _collectedItems)
            {
                var history = _itemHistory[item.Data.ItemVariation];
                history.HighestLevel = Mathf.Max(history.HighestLevel, item.CurrentLevel);
            }

            float currentTime = Time.timeSinceLevelLoad;
            List<ItemStatistics> itemStatisticsList = new(_itemHistory.Count);
            foreach (var history in _itemHistory.Values)
            {
                float dps = history.Data.Cooldown > 0f ? history.Data.Damage / history.Data.Cooldown : 0f;
                itemStatisticsList.Add(new ItemStatistics(
                    history.Data,
                    history.TotalDamage,
                    history.HighestLevel,
                    dps,
                    TimeSpan.FromSeconds(history.GetSecondsInInventory(currentTime)),
                    history.EquippedSince.HasValue));
            }

            return itemStatisticsList;
        }

        public void StopTrackingItems()
        {
            foreach (var item in _trackedItems)
                item.DamageDealt -= OnItemDamageDealt;

            _trackedItems.Clear();

            foreach (var item in _collectedItems)
                item.MaxLevelReached -= OnItemMaxLevelReached;
        }

        private void OnItemDamageDealt(ItemVariations variation, float damage)
        {
            if (damage > 0f && _itemHistory.TryGetValue(variation, out var history))
                history.TotalDamage += damage;
        }

        private sealed class ItemHistory
        {
            public ItemHistory(ItemDataConfig data)
            {
                Data = data;
            }

            public ItemDataConfig Data { get; }
            public float TotalDamage { get; set; }
            public int HighestLevel { get; set; }
            public float SecondsInInventory { get; set; }
            public float? EquippedSince { get; set; }

            public float GetSecondsInInventory(float currentTime)
            {
                return EquippedSince is float startTime
                    ? SecondsInInventory + Mathf.Max(0f, currentTime - startTime)
                    : SecondsInInventory;
            }
        }
    }
}