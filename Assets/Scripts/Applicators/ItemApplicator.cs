using Applicators;
using Data;
using Items.ItemData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Applicators_
{
    public class ItemApplicator : BaseApplicator<ItemVisualData>
    {
        [SerializeField] private StatApplicator _statPrefab;
        [SerializeField] private Transform _statContainer;
        [SerializeField] private Button _ok;

        private List<StatApplicator> _currentStats = new ();

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

            //добавление характеристик, пока не знаю, как они реализованы, нужно доделать
            ItemDataConfig config = data.Config;

            AddStat(nameof(config.Damage), config.Damage);
            AddStat(nameof(config.Cooldown), config.Cooldown);
            AddStat(nameof(config.Rarity), config.Rarity);
        }

        private void AddStat(string name, float value)
        {
            StatApplicator stat = Instantiate(_statPrefab, _statContainer);
            stat.Applicate(name, value);
            _currentStats.Add(stat);
        }

        private void RemoveAllStat()
        {
            foreach (var stat in _currentStats)
                Destroy(stat.gameObject);

            _currentStats.Clear();
        }

        private void AddItem()
        {
            //тут нужно добавить текущий итем CurrentItem в инвентарь. Но тут есть только ItemDataConfig, а в инвентарь добавляются Item. Пока хз, как создать Item
            
        }
    }
}