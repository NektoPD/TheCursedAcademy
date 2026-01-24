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
            List<ItemVisualData> visualDatasInInventory = _inventory.Items.Select(item => item.VisualData).ToList();

            _applicator.SetDefaultItem(visualDatasInInventory.First());
            _applicator.Inizialize(visualDatasInInventory);
            _applicator.Initialize(_inventory.Items);

            base.OpenWindow();

            for (int i = 0; i < _itemsVisual.Count; i++)
            {
                if (_inventory.Items.Count - 1 >= i)
                {
                    if (_inventory.Items[i] == null)
                        continue;

                    int level = _inventory.Items[i] ? 0 : _inventory.Items[i].CurrentLevel;
                    _itemsVisual[i].gameObject.SetActive(true);
                    _itemsVisual[i].Initialize(_inventory.Items[i].VisualData, false, level);
                    continue;
                }

                _itemsVisual[i].gameObject.SetActive(false);
            }
        }
    }
}