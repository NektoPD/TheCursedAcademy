using PickableItems;
using Zenject;

namespace Pools.FromPrefab
{
    public class MagnetPool : PoolFromPrefab<Magnet>
    {
        public MagnetPool(DiContainer container, Magnet prefab) : base(container, prefab) { }

        protected override void Initialize(Magnet entity, int count) => entity.Initialize(count, this);

        protected override Magnet Instantiate(Magnet prefab) => Container.InstantiatePrefabForComponent<Magnet>(prefab);
    }
}