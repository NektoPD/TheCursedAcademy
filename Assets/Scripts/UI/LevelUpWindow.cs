using Data;
using InventorySystem;
using Items.BaseClass;
using Items.ItemHolder;
using System.Collections.Generic;
using System.Linq;
using UI.Applicators;
using UnityEngine;
using Zenject;

namespace UI 
{
    public class LevelUpWindow : Window
    {
        private const int CountItems = 3;

        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicator _applicator;

        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        public void Initialize(CharacterInventory inventory) => _inventory = inventory;

        public override void OpenWindow()
        {
            IEnumerable<ItemVisualData> visualDatasInInventory = _inventory.Items.Select(item => item.VisualData);
            _itemsHolder.GetVisualDatas(CountItems, out List<ItemVisualData> datas);

            _applicator.SetDefaultItem(datas.First());
            _applicator.Inizialize(visualDatasInInventory);

            base.OpenWindow();

            for (int i = 0; i < CountItems; i++) 
            {
                bool isNew = !visualDatasInInventory.Contains(datas[i]);
                Item item = _inventory.Items.Where(item => item.VisualData == datas[i]).FirstOrDefault();
                int level = item == null ? 0 : item.CurrentLevel;
                _itemsVisual[i].Initialize(datas[i], isNew, level);
            }
        }
    }
}