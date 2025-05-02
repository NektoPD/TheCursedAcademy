using Zenject;
using PlayerPerksController;
using WalletSystem;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PerkController>().AsSingle().NonLazy();
            Container.Bind<Wallet>().AsSingle().NonLazy();
        }
    }
}