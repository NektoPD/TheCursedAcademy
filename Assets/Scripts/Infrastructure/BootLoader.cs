using PlayerPerksController;
using TheraBytes.BetterUi;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using WalletSystem;
using UnityEditor;

namespace Infrastructure
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private SceneAsset _scene;

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
            SceneManager.LoadScene(_scene.name);
            ResolutionMonitor.EnsureInstance();
        }
    }
}