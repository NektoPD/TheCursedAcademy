using CharacterLogic.Initializer;
using EnemyLogic;
using Timelines;
using UnityEngine;
using Zenject;

public class TimelineInstaller : MonoInstaller
{
    [SerializeField] private CharacterInitializer _initializer;

    public override void InstallBindings()
    {
        Container.BindInstance(_initializer).WhenInjectedInto<TimelineCharacterController>();
        Container.BindInstance(_initializer).WhenInjectedInto<EnemyDamageTaker>();
    }
}