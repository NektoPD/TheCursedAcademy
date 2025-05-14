using PickableItems;
using Zenject;

namespace Pools.FromPrefab
{
    public class MoneyPool : PoolFromPrefab<Money>
    {
        public MoneyPool(DiContainer container, Money prefab) : base(container, prefab) { }

        protected override void Initialize(Money entity, int count) =>  entity.Initialize(count, this);
     

        protected override Money Instantiate(Money prefab) => Container.InstantiatePrefabForComponent<Money>(prefab);
    }
}
