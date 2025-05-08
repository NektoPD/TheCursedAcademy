using CharacterLogic.Initializer;
using UnityEngine;
using Zenject;
using UI;
using StatistiscSystem;

public class WindowsInstaller : MonoInstaller
{
    [SerializeField] private CharacterInitializer _initializer;

    public override void InstallBindings()
    {
        Container.BindInstance(_initializer).WhenInjectedInto<ExitToMenu>();
        Container.BindInstance(_initializer).WhenInjectedInto<Reviver>();
        Container.BindInstance(_initializer).WhenInjectedInto<StatisticsApplicator>();
        Container.BindInstance(_initializer).WhenInjectedInto<LevelUpWindow>();

    }
}
