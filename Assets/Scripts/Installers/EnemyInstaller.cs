using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private List<EnemyData> _enemyDataList;

    public override void InstallBindings()
    {
        Container.Bind<EnemyPool>().AsSingle();

        Container.Bind<EnemySpawner>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<Difficulty>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
    }
}