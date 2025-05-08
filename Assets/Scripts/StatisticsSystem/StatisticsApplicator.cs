using CharacterLogic.Initializer;
using TMPro;
using UI;
using UnityEngine;
using Zenject;

namespace StatistiscSystem
{
    public class StatisticsApplicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _totalScore;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _coins;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private SpriteSetter _itemPrefab;
        [SerializeField] private ItemStatisticsApplicator _itemStatisticsApplicator;
        [SerializeField] private EndWindow _window;
        
        private CharacterInitializer _characterInitializer;

        [Inject]
        private void Construct(CharacterInitializer characterInitializer)
        {
            _characterInitializer = characterInitializer;
        }

        private void OnEnable()
        {
            _characterInitializer.Character.StatisticCollected += Applicate;
        }

        private void OnDisable()
        {
            _characterInitializer.Character.StatisticCollected -= Applicate;
        }

        private void Applicate(Statistics statistics)
        {
            _totalScore.text = statistics.TotalScore.ToString();
            _level.text = statistics.Level.ToString();
            _coins.text = statistics.Coins.ToString();
            _time.text = statistics.LiveTime.ToString(@"hh\:mm\:ss");

            foreach (var item in statistics.Items)
            {
                var itemView = Instantiate(_itemPrefab, _itemsContainer);
                itemView.SetSprite(item.Item.ItemIcon);
            }

            _itemStatisticsApplicator.Applicate(statistics);

            _window.OpenWindow();
        }
    }
}