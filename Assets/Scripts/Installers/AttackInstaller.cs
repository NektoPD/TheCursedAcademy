using Zenject;
using EnemyLogic.Attackers;

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