using PlayerPerksController;
using TheraBytes.BetterUi;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class BootLoader : MonoBehaviour
    {
        private PerkController _perkController;

        [Inject]
        public void Construct(PerkController controller)
        {
            _perkController = controller;
            _perkController.Initialize();
        }

        private void Start()
        {
            SceneManager.LoadScene("Menu");
            ResolutionMonitor.EnsureInstance();
        }
    }
}