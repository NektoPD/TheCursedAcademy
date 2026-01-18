using System.Collections.Generic;
using System.Linq;
using Data;
using InventorySystem;
using Items.BaseClass;
using Items.ItemHolder;
using UI.Applicators;
using UnityEngine;
using Zenject;

namespace UI
{
    public class InventoryFullWindow : Window
    {
        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicator _applicator;

        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;
        private int _countItems;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        public void Initialize(CharacterInventory inventory)
        {
            _inventory = inventory;

            _countItems = _inventory.InventoryLimit;
            
            for (var i = 0; i < _itemsVisual.Count; i++)
            {
                bool gameObjectStatus = i + 1 <= _countItems;
                _itemsVisual[i].gameObject.SetActive(gameObjectStatus);
            }
        }

        public override void OpenWindow()
        {
            IEnumerable<ItemVisualData> visualDatasInInventory = _inventory.Items.Select(item => item.VisualData);

            var datasInInventory = visualDatasInInventory as ItemVisualData[] ?? visualDatasInInventory.ToArray();
            _applicator.SetDefaultItem(datasInInventory.First());
            _applicator.Inizialize(datasInInventory);
            _applicator.Initialize(_inventory.Items);

            base.OpenWindow();

            for (int i = 0; i < _countItems; i++)
            {
                Item item = _inventory.Items.FirstOrDefault(item => item.VisualData == visualDatasInInventory);
                int level = item == null ? 0 : item.CurrentLevel;
                _itemsVisual[i].Initialize(datasInInventory[i], false, level);
            }
        }
    }
}