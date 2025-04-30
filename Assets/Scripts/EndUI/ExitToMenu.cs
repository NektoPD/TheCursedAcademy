using CharacterLogic.Initializer;
using StatistiscSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using WalletSystem;
using YG;
using Zenject;

namespace EndUI
{
    public class ExitToMenu : MonoBehaviour
    {
        [SerializeField] private Button _exit;
        [SerializeField] private Button _adsExit;
        [SerializeField] private SceneAsset _menuScene;
        [SerializeField] private Ads _ads;

        private int _coins;
        private bool _isShowing = false;
        private Wallet _wallet;
        private IStatisticsTransmitter _transmitter;

        [Inject]
        public void Construct(Wallet wallet, CharacterInitializer characterInitializer)
        {
            _wallet = wallet;
            _transmitter = characterInitializer.Character;
        }

        private void OnEnable()
        {
            _transmitter.StatisticCollected += Initialize;
            _exit.onClick.AddListener(Exit);
            _adsExit.onClick.AddListener(Ads);
            YandexGame.RewardVideoEvent += OnRevardedShow;
        }

        private void OnDisable()
        {
            _transmitter.StatisticCollected -= Initialize;
            _exit.onClick.RemoveListener(Exit);
            _adsExit.onClick.RemoveListener(Ads);
            YandexGame.RewardVideoEvent -= OnRevardedShow;
        }

        public void Initialize(Statistics statistics) => _coins = statistics.Coins;

        private void Ads()
        {
            _ads.OpenRewardAd();
            _isShowing = true;
        }

        private void Exit()
        {
            SceneManager.LoadScene(_menuScene.name);
        }

        private void OnRevardedShow(int obj)
        {
            if (_isShowing == false)
                return;

            _wallet.AddMoney(_coins);
            Exit();
        }
    }
}
