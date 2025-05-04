using PlayerPerksController;
using TheraBytes.BetterUi;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using WalletSystem;
using UnityEditor;
using CharacterLogic.Data;
using YG;

namespace Infrastructure
{
    public class BootLoader : MonoBehaviour
    {
        private const string Key = "CharacterId";

        [SerializeField] private SceneAsset _menu;
        [SerializeField] private SceneAsset _tutorial;
        [SerializeField] private CharacterData.CharacterType _type;

        private PerkController _perkController;
        private Wallet _wallet;

        [Inject]
        public void Construct(PerkController controller, Wallet wallet)
        {
            _perkController = controller;
            _wallet = wallet;

            _perkController.Initialize();
            _wallet.Initialize();
        }

        private void Start()
        {
            PlayerPrefs.SetInt(Key, (int)_type);
            ResolutionMonitor.EnsureInstance();

            if (YandexGame.savesData.isFirstSession == false)
                SceneManager.LoadScene(_menu.name);

            YandexGame.savesData.isFirstSession = false;
            YandexGame.SaveProgress();

            SceneManager.LoadScene(_tutorial.name);
        }
    }
}