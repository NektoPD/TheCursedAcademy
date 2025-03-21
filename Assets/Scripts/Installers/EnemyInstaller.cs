using Pools;
using Zenject;
using EnemyLogic;
using UnityEngine;
using EnemyLogic.Attackers;
using EnemyLogic.HealthBars;
using CharacterLogic.Initializer;

namespace Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private Transform _healthBarsContainer;

        public override void InstallBindings()
        {
            Container.Bind<EnemyPool>().AsSingle();

            Container.BindInstance(_initializer).WhenInjectedInto<RangeAttacker>();
            Container.BindInstance(_initializer).WhenInjectedInto<EnemyMover>();
            Container.BindInstance(_healthBarsContainer).WhenInjectedInto<BossHealthBar>();
        }
    }
}