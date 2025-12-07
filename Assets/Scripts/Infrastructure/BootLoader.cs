using PlayerPerksController;
using TheraBytes.BetterUi;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using WalletSystem;
using CharacterLogic.Data;
using Data;
using YG;

namespace Infrastructure
{
    public class BootLoader : MonoBehaviour
    {
        private const string Key = "CharacterId";
        private const string NewPlayerKey = "NewPlayerKey";

        [SerializeField] private int _menuIdSecene;
        [SerializeField] private int _tutorialIdSecene;
        [SerializeField] private CharacterData.CharacterType _type;

        private PerkController _perkController;
        private Wallet _wallet;
        
        [Inject]
        public void Construct(PerkController controller, Wallet wallet)
        {
            _perkController = controller;
            _wallet = wallet;
        }

        private void Start()
        {
            PlayerPrefs.SetInt(Key, (int)_type);
            ResolutionMonitor.EnsureInstance();

            SceneManager.LoadScene(YandexGame.savesData.IsTutorialCompleted ? _menuIdSecene : _tutorialIdSecene);

            YandexGame.SaveProgress();
            
            _perkController.Initialize();
            _wallet.Initialize();
        }
    }
}