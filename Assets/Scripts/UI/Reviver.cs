using CharacterLogic;
using CharacterLogic.Initializer;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG;
using Zenject;

namespace UI
{
    public class Reviver : MonoBehaviour
    {
        private const string Close = nameof(Close);

        [SerializeField] private Button _revive;
        [SerializeField] private Ads _ads;
        [SerializeField] private EndWindow _endWindow;
        [SerializeField] private Animator _animator;

        private Character _character;
        private bool _isShowing = false;

        [Inject]
        public void Construct(CharacterInitializer characterInitializer)
        {
            _character = characterInitializer.Character;
        }

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

        private void Ads()
        {
            _ads.OpenRewardAd();
            _isShowing = true;
        }

        private void Revive(int obj)
        {
            if (_isShowing == false)
                return;

            _animator.SetTrigger(Close);
            _character.Revive();
        }
    }
}