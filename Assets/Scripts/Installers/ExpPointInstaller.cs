using Pools;
using Zenject;

namespace Installers
{
    public class ExpPointInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ExpPointPool>().AsSingle();
        }
    }
}