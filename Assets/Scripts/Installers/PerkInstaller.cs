using PlayerPerksController;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class PerkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PerkController>().AsSingle().NonLazy();
        }
    }
}