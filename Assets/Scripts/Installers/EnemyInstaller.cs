using CharacterLogic.Initializer;
using UnityEngine;
using Zenject;

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