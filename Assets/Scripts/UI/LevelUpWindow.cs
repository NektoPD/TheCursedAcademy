using Data;
using System.Collections.Generic;
using System.Linq;
using UI.Applicators;
using UnityEngine;

namespace UI 
{ 
    public class LevelUpWindow : Window
    {
        private const int CountItems = 3;

        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicator _applicator;

        public void ShowWithItems(List<ItemVisualData> items)
        {
            for (int i = 0; i < CountItems; i++)
                _itemsVisual[i].Initialize(items[i]);

            _applicator.SetdDefaultItem(items.First());
            Show();
        }
    }
}