using CharacterLogic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using WalletSystem;
using YG;
using Zenject;

namespace UI
{
    public class ExitToMenu : MonoBehaviour
    {
        [SerializeField] private Button _exit;
        [SerializeField] private Button _adsExit;
        [SerializeField] private SceneAsset _menuScene;
        [SerializeField] private Ads _ads;
        [SerializeField] private SceneChanger _changer;

        private int _coins;
        private bool _isShowing = false;
        private Wallet _wallet;

        [Inject]
        public void Construct(Wallet wallet)
        {
            _wallet = wallet;
        }

        private void OnEnable()
        {
            _exit.onClick.AddListener(Exit);
            _adsExit.onClick.AddListener(Ads);
            YandexGame.RewardVideoEvent += OnRevardedShow;
        }

        private void OnDisable()
        {
            _exit.onClick.RemoveListener(Exit);
            _adsExit.onClick.RemoveListener(Ads);
            YandexGame.RewardVideoEvent -= OnRevardedShow;
        }

        public void SetCoins(int coins) => _coins = coins;

        private void Ads()
        {
            _ads.OpenRewardAd();
            _isShowing = true;
        }

        private void Exit()
        {
            _changer.ChangeScene(_menuScene);
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
