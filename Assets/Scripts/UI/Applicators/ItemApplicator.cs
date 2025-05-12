using Data;
using InventorySystem;
using Items.Enums;
using Items.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators
{
    public class ItemApplicator : BaseApplicator<ItemVisualData>
    {
        private readonly List<StatView> _currentStats = new ();

        [SerializeField] private StatView _statPrefab;
        [SerializeField] private Transform _statContainer;
        [SerializeField] private Button _ok;

        private IEnumerable<ItemVisualData> _visualDatasInInventory;

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

        public void Inizialize(IEnumerable<ItemVisualData> visualDatasInInventory) => _visualDatasInInventory = visualDatasInInventory;

        protected override void Applicate(ItemVisualData data)
        {
            if(data == null)
                return;

            RemoveAllStat();


            foreach (var stat in data.Stats)
                if (_visualDatasInInventory.Contains(data))
                    AddStat(stat.Name, $"{stat.CurrentValue} -> {stat.NextValue}");
                else
                    AddStat(stat.Name, $"0 -> {stat.CurrentValue}");
        }
        
        private void AddStat(string name, string value)
        {
            StatView stat = Instantiate(_statPrefab, _statContainer);
            stat.Applicate(name, value.ToString());
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