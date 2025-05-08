using Data;
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

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        public override void Show()
        {
            base.Show();

            List<ItemVisualData> datas = new();

            _itemsHolder.GetVisualDatas(CountItems, out datas);

            for (int i = 0; i < CountItems; i++)
                _itemsVisual[i].Initialize(datas[i]);

            _applicator.SetdDefaultItem(datas.First());
        }
    }
}