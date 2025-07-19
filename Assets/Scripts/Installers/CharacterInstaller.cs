using CharacterLogic;
using CharacterLogic.Initializer;
using EnemyLogic;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField] private Character _characterPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<Character, CharacterFabric>()
                     .FromComponentInNewPrefab(_characterPrefab);
            
            Container.Bind<KilledEnemyCounter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
