using Data;
using Items.ItemData;
using System.Collections.Generic;
using UnityEngine;

namespace UI 
{ 
    public class LevelUpWindow : Window
    {
        [SerializeField] private List<ItemVisualData> _itemsVisual;

        public void ShowWithItems(List<ItemDataConfig> items)
        {
            for (int i = 0; i < items.Count; i++)
                _itemsVisual[i].Initialize(items[i]);

            Show();
        }
    }
}