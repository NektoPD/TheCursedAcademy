using System.Collections.Generic;
using UnityEngine;

namespace StatistiscSystem
{
    public class ItemStatisticsApplicator : MonoBehaviour
    {
        [SerializeField] private Transform _itemStatisticsContainer;
        [SerializeField] private ItemStatisticView _prefab;

        public void Applicate(Statistics statistics)
        {
            if (statistics?.Items == null)
                return;

            foreach (var item in statistics.Items)
            {
                var itemView = Instantiate(_prefab, _itemStatisticsContainer);
                itemView.View(item.Item.ItemIcon, item.TotalDamage.ToString(), item.Level.ToString(), item.DPS.ToString(), FormatTime(item.TimeInInventory));
            }
        }
        
        public void Applicate(IReadOnlyList<ItemStatistics> itemStatisticsList)
        {
            foreach (Transform child in _itemStatisticsContainer)
            {
                Destroy(child.gameObject);
            }
            
            if (itemStatisticsList == null)
                return;

            foreach (var item in itemStatisticsList)
            {
                var itemView = Instantiate(_prefab, _itemStatisticsContainer);
                itemView.View(item.Item.ItemIcon, item.TotalDamage.ToString(), item.Level.ToString(), item.DPS.ToString(), FormatTime(item.TimeInInventory));
            }
        }

        private static string FormatTime(System.TimeSpan time)
        {
            return time.TotalDays >= 1
                ? time.ToString(@"d\:hh\:mm\:ss")
                : time.ToString(@"hh\:mm\:ss");
        }
    }
}