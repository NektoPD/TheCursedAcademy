using TMPro;
using UI.Animation;
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

        public void Applicate(Statistics statistics)
        {
            _totalScore.text = statistics.TotalScore.ToString();
            _level.text = statistics.Level.ToString();
            _coins.text = statistics.Coins.ToString();
            _enemyKilled.text = statistics.EnemysKills.ToString();
            _time.text = statistics.LiveTime.ToString(@"hh\:mm\:ss");

            foreach (var item in statistics.Items)
            {
                var itemView = Instantiate(_itemPrefab, _itemsContainer);
                itemView.SetSprite(item.Item.ItemIcon);
            }

            _itemStatisticsApplicator.Applicate(statistics);

            _window.Open();
            _window.StopTime();
        }
    }
}