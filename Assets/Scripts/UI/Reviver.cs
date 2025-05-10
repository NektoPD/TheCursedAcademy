using CharacterLogic;
using UI.Animation;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG;

namespace UI
{
    public class Reviver : MonoBehaviour
    {
        [SerializeField] private Button _revive;
        [SerializeField] private Ads _ads;
        [SerializeField] private WindowAnimation _window;

        private Character _character;
        private bool _isShowing = false;

        private void OnEnable()
        {
            _revive.onClick.AddListener(Ads);
            YandexGame.RewardVideoEvent += Revive;
        }

        private void OnDisable()
        {
            _revive.onClick.RemoveListener(Ads);
            YandexGame.RewardVideoEvent -= Revive;
        }

        public void Inizialize(Character character) => _character = character;

        private void Ads()
        {
            _ads.OpenRewardAd();
            _isShowing = true;
        }

        private void Revive(int obj)
        {
            if (_isShowing == false)
                return;

            _window.Close();
            _window.StartTime();
            _character.Revive();
        }
    }
}