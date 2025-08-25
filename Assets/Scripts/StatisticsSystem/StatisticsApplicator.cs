using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI.Animation;
using Unity.VisualScripting;
using UnityEngine;

namespace StatistiscSystem
{
    public class StatisticsApplicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _totalScore;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _coins;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _enemyKilled;
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private SpriteSetter _itemPrefab;
        [SerializeField] private ItemStatisticsApplicator _itemStatisticsApplicator;
        [SerializeField] private WindowAnimation _window;

        private HashSet<ItemStatistics> _displayedItems = new HashSet<ItemStatistics>();
        
        public void Applicate(Statistics statistics)
        {
            ClearContainers();
            
            _displayedItems.Clear();
            
            _totalScore.text = statistics.TotalScore.ToString();
            _level.text = statistics.Level.ToString();
            _coins.text = statistics.Coins.ToString();
            _enemyKilled.text = statistics.EnemysKills.ToString();
            _time.text = statistics.LiveTime.ToString(@"hh\:mm\:ss");
            
            foreach (var item in statistics.Items)
            {
                _displayedItems.Add(item);
            }
            
            foreach (var item in _displayedItems)
            {
                var itemView = Instantiate(_itemPrefab, _itemsContainer);
                itemView.SetSprite(item.Item.ItemIcon);
            }

            _itemStatisticsApplicator.Applicate(_displayedItems);

            _window.Open();
            _window.StopTime();
        }
        
        private void ClearContainers()
        {
            foreach (Transform child in _itemsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}