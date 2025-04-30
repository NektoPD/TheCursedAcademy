using CharacterLogic.Initializer;
using UnityEngine;
using Zenject;
using EndUI;

public class EndWindowInstaller : MonoInstaller
{
    [SerializeField] private CharacterInitializer _initializer;

    public override void InstallBindings()
    {
        Container.BindInstance(_initializer).WhenInjectedInto<ExitToMenu>();
        Container.BindInstance(_initializer).WhenInjectedInto<Reviver>();
    }
}
