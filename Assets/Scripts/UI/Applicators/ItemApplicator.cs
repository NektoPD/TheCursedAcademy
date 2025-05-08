using Data;
using Items.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators
{
    public class ItemApplicator : BaseApplicator<ItemVisualData>
    {
        [SerializeField] private StatView _statPrefab;
        [SerializeField] private Transform _statContainer;
        [SerializeField] private Button _ok;

        private readonly List<StatView> _currentStats = new ();

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

        protected override void Applicate(ItemVisualData data)
        {
            RemoveAllStat();

            foreach (var stat in data.Stats)
                AddStat(stat.Name, $"{stat.PastValue} -> {stat.CurrentValue}");
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