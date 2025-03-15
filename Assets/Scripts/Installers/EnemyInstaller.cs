using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _healthBarsContainer;

    public override void InstallBindings()
    {
        Container.Bind<EnemyPool>().AsSingle();

        Container.BindInstance(_target).WhenInjectedInto<RangeAttacker>();
        Container.BindInstance(_target).WhenInjectedInto<EnemyMover>();
        Container.BindInstance(_target).WhenInjectedInto<EnemyLayerOrder>();
        Container.BindInstance(_healthBarsContainer).WhenInjectedInto<BossHealthBar>();
    }
}