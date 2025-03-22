using Data;
using Zenject;
using Difficulties;
using UnityEngine;
using System.Collections.Generic;

namespace Installers
{
    public class DifficultyInstaller : MonoInstaller
    {
        [SerializeField] private List<EnemyData> _enemyDataList;

        public override void InstallBindings()
        {
            Container.BindInstance(_enemyDataList).WhenInjectedInto<Difficulty>();
        }
    }
}