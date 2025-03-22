using Data;
using EnemyLogic;
using Zenject;

namespace Pools
{
    public class EnemyPool : Pool<Enemy>
    {
        public EnemyPool(DiContainer container) : base(container) { }

        protected override Enemy Create(IData<Enemy> data) => Container.InstantiatePrefabForComponent<Enemy>(data.Prefab);

        protected override Enemy Initialize(IData<Enemy> data, Enemy entity)
        {
            entity.Initialize(data, this);
            entity.ResetEntity();
            return entity;
        }
    }
}
