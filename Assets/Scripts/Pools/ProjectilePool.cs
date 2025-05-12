using Data;
using EnemyLogic.ProjectileLogic;
using System.Linq;
using Zenject;

namespace Pools
{
    public class ProjectilePool : Pool<Projectile>
    {
        public ProjectilePool(DiContainer container) : base(container) { }

        protected override Projectile Create(IData<Projectile> data) => Container.InstantiatePrefabForComponent<Projectile>(data.Prefab);

        protected override Projectile Initialize(IData<Projectile> data, Projectile entity)
        {
            entity.Initialize(data, this);
            entity.ResetEntity();
            return entity;
        }

        protected override bool TryGetInPool(IData<Projectile> data, out Projectile entity)
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