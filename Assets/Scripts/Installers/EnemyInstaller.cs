using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private List<EnemyData> _enemyDataList;
    [SerializeField] private Transform _target;

    public override void InstallBindings()
    {
        Container.Bind<EnemyPool>().AsSingle();

        Container.BindInstance(_target).WhenInjectedInto<EnemyMover>();
        Container.BindInstance(_target).WhenInjectedInto<LayerOrderController>();

        Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
    }
}