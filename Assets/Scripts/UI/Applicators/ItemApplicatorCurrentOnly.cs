using Data;
using InventorySystem;
using Items.Enums;
using Items.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using Items.BaseClass;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators
{
    public class ItemApplicatorCurrentOnly : BaseApplicator<ItemVisualData>
    {
        private readonly List<StatView> _currentStats = new();

        [SerializeField] private StatView _statPrefab;
        [SerializeField] private Transform _statContainer;
        [SerializeField] private Button _ok;

        private IEnumerable<ItemVisualData> _visualDatasInInventory;

        public void Inizialize(IEnumerable<ItemVisualData> visualDatasInInventory) =>
            _visualDatasInInventory = visualDatasInInventory;

        public event Action<ItemVariations> ItemSelected;

        protected override void OnEnable()
        {
            base.OnEnable();
            _ok.onClick.AddListener(AddItem);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _ok.onClick.RemoveListener(AddItem);
        }

        private IReadOnlyDictionary<ItemVisualData, Items.BaseClass.Item> _itemsByVisual;

        public void Initialize(IEnumerable<Item> itemsInInventory)
        {
            _itemsByVisual = itemsInInventory.ToDictionary(i => i.VisualData, i => i);
        }

        protected override void Applicate(ItemVisualData data)
        {
            if (data == null) return;

            RemoveAllStat();

            Item item = null;
            bool owned = _itemsByVisual != null && _itemsByVisual.TryGetValue(data, out item);

            var stats = owned
                ? item.UiStats
                : data.Stats;

            foreach (var stat in stats)
            {
                float current = Round2(stat.CurrentValue);

                AddStat(
                    stat.Name,
                    $"{current:F2}"
                );
            }
        }

        private static float Round2(float value)
        {
            return (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private void AddStat(string name, string value)
        {
            StatView stat = Instantiate(_statPrefab, _statContainer);
            stat.Applicate(name, value);
            _currentStats.Add(stat);
        }

        private void RemoveAllStat()
        {
            foreach (var stat in _currentStats)
                Destroy(stat.gameObject);

            _currentStats.Clear();
        }

        private void AddItem() => ItemSelected?.Invoke(CurrentItem.Variation);
    }
}
