using Pools;
using Zenject;

namespace Installers
{
    public class ProjectileInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ProjectilePool>().AsSingle();
        }
    }
}