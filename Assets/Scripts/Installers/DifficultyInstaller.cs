using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DifficultyInstaller : MonoInstaller
{
    [SerializeField] private List<EnemyData> _enemyDataList;

    public override void InstallBindings()
    {
        Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
    }
}