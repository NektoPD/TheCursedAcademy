using Data;
using ExpPoints;
using System.Linq;
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

        protected override bool TryGetInPool(IData<ExpPoint> data, out ExpPoint entity)
        {
            entity = null;

            var currentEntity = EntityPool.Where(entity => entity.Prefab == data.Prefab);

            if (currentEntity.Count() > 0)
            {
                entity = currentEntity.First();
                entity = Initialize(data, entity);
                EntityPool.Remove(entity);

                return true;
            }

            return false;
        }
    }
}
