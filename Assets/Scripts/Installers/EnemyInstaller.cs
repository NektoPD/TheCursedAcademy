using CharacterLogic.Initializer;
using EnemyLogic;
using EnemyLogic.Attackers;
using EnemyLogic.HealthBars;
using Pools;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private Transform _healthBarsContainer;
        [SerializeField] private AudioSource _deathSound;

        public override void InstallBindings()
        {
            Container.Bind<EnemyPool>().AsSingle();

            Container.BindInstance(_deathSound).AsSingle();
            Container.BindInstance(_initializer).WhenInjectedInto<RangeAttacker>();
            Container.BindInstance(_initializer).WhenInjectedInto<EnemyMover>();
            Container.BindInstance(_healthBarsContainer).WhenInjectedInto<BossHealthBar>();
        }
    }
}