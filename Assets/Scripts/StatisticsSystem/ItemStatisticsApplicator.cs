using UnityEngine;

namespace StatistiscSystem
{
    public class ItemStatisticsApplicator : MonoBehaviour
    {
        [SerializeField] private Transform _itemStatisticsContainer;
        [SerializeField] private ItemStatisticView _prefab;

        public void Applicate(Statistics statistics)
        {
            foreach (var item in statistics.Items)
            {
                var itemView = Instantiate(_prefab, _itemStatisticsContainer);
                itemView.View(item.Item.ItemIcon, item.TotalDamage.ToString(), item.Level.ToString(), item.DPS.ToString(), item.TimeInInventory.ToString());
            }
        }
    }
}