using UnityEngine;
using Zenject;

namespace Installers
{
    public class TutorialEnemyInstaller : MonoInstaller
    {
        [SerializeField] private AudioSource _tutorialDeathSound;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_tutorialDeathSound).AsSingle();
        }
    }
}