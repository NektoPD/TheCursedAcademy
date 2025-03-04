using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private List<EnemyData> _enemyDataList;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _healthBarsContainer;

    public override void InstallBindings()
    {
        Container.Bind<EnemyPool>().AsSingle();
        Container.Bind<MeleeAttacker>().AsSingle();
        Container.Bind<RangeAttacker>().AsSingle();
        Container.Bind<SpawnAttacker>().AsSingle();
        Container.Bind<AttackerManager>().AsSingle();

        Container.BindInstance(_target).WhenInjectedInto<RangeAttacker>();
        Container.BindInstance(_target).WhenInjectedInto<EnemyMover>();
        Container.BindInstance(_target).WhenInjectedInto<LayerOrderController>();
        Container.BindInstance(_healthBarsContainer).WhenInjectedInto<BossHealthBar>();
        Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
    }
}