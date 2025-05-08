using UI.Applicators;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ItemApplicatorInstaller : MonoInstaller
    {
        [SerializeField] private ItemApplicator _applicator;

        public override void InstallBindings()
        {
            Container.BindInstance(_applicator);
        }
    }
}