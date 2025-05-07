using Items.ItemHolder;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ItemHolderInstaller : MonoInstaller
    {
        [SerializeField] private ItemsHolder _itemsHolder;

        public override void InstallBindings()
        {
            var itemHolder = Container.InstantiatePrefabForComponent<ItemsHolder>(_itemsHolder);

            Container.Bind<ItemsHolder>().FromInstance(itemHolder).AsSingle();
        }
    }
}