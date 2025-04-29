using PlayerPerksController;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using WalletSystem;

namespace Infrastructure
{
    public class BootLoader : MonoBehaviour
    {
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
            SceneManager.LoadScene("Menu");
        }
    }
}