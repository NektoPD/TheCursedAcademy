using CharacterLogic;
using CharacterLogic.Initializer;
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

        private CharacterInitializer _initializer;

        private bool _isShowing = false;
        private bool _deathPauseHeld;

        private void OnEnable()
        {
            _revive.onClick.AddListener(Ads);
            YandexGame.RewardVideoEvent += Revive;
        }

        private void OnDisable()
        {
            _revive.onClick.RemoveListener(Ads);
            YandexGame.RewardVideoEvent -= Revive;
            ReleaseDeathPause();
        }

        public void Inizialize(Character character, CharacterInitializer initializer)
        {
            _character = character;
            _initializer = initializer;

            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (_revive == null) return;

            bool canRevive = _initializer != null && _initializer.WasRevivedThisSession == false;
            _revive.interactable = canRevive;
        }

        public void HoldDeathPause()
        {
            if (_deathPauseHeld)
                return;

            _deathPauseHeld = true;
            GamePauseController.HoldPause();
        }

        private void Ads()
        {
            if (_initializer != null && _initializer.WasRevivedThisSession)
            {
                UpdateButtonState();
                return;
            }

            _ads.OpenRewardAd();
            _isShowing = true;
        }

        private void Revive(int obj)
        {
            if (_isShowing == false)
                return;

            if (_initializer != null && _initializer.WasRevivedThisSession)
            {
                UpdateButtonState();
                return;
            }

            _window.Close();
            _character.Revive();
            ReleaseDeathPause();

            if (_initializer != null)
                _initializer.MarkRevived();

            _isShowing = false;
            UpdateButtonState();
        }

        private void ReleaseDeathPause()
        {
            if (!_deathPauseHeld)
                return;

            _deathPauseHeld = false;
            GamePauseController.ReleasePause();
        }
    }
}