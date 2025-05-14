using PickableItems;
using Zenject;

namespace Pools.FromPrefab
{
    public class HealPool : PoolFromPrefab<Heal>
    {
        public HealPool(DiContainer container, Heal prefab) : base(container, prefab) { }

        protected override void Initialize(Heal entity, int count) => entity.Initialize(count, this);

        protected override Heal Instantiate(Heal prefab) => Container.InstantiatePrefabForComponent<Heal>(prefab);
    }
}