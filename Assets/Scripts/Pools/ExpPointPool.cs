using Data;
using ExpPoints;
using Zenject;

namespace Pools
{
    public class ExpPointPool : Pool<ExpPoint>
    {
        public ExpPointPool(DiContainer container) : base(container) { }

        protected override ExpPoint Create(IData<ExpPoint> data) => Container.InstantiatePrefabForComponent<ExpPoint>(data.Prefab);

        protected override ExpPoint Initialize(IData<ExpPoint> data, ExpPoint entity)
        {
            entity.Initialize(data, this);
            entity.ResetEntity();
            return entity;
        }
    }
}
