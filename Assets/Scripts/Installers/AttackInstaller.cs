using EnemyLogic.Attackers;
using Zenject;

namespace Installers
{
    public class AttackInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeleeAttacker>().AsSingle();
            Container.Bind<RangeAttacker>().AsSingle();
            Container.Bind<SpawnAttacker>().AsSingle();
            Container.Bind<AttackerManager>().AsSingle();
        }
    }
}