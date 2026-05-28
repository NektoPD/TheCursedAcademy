using Pools;
using Zenject;
using Difficulties;

namespace Installers
{
    public class ExpPointInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<XpWaveScaler>().AsSingle();
            Container.Bind<ExpPointPool>().AsSingle();
        }
    }
}