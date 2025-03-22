using Data;
using EnemyLogic.ProjectileLogic;
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
    }
}