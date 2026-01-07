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
            entity = EntityPool.FirstOrDefault(e => e.Prefab == data.Prefab);
            if (entity == null)
                return false;

            EntityPool.Remove(entity);
            return true;
        }
    }
}