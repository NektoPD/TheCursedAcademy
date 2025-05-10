using Data.EnemesData;
using Difficulties;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class DifficultyInstaller : MonoInstaller
    {
        [SerializeField] private List<EnemyData> _enemyDataList;

        public override void InstallBindings()
        {
            Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
            Container.BindInstance(_enemyDataList).WhenInjectedInto<EventStarter>();
        }
    }
}