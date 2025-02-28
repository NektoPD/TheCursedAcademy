using System.Linq;
using Zenject;

public class ProjectilePool : Pool<Projectile>
{
    public ProjectilePool(DiContainer container) : base(container) { }

    protected override Projectile Create(IData<Projectile> data)
    {
        Projectile newProjectile = Container.InstantiatePrefabForComponent<Projectile>(data.Prefab);
        newProjectile.Initialize(data, this);
        return newProjectile;
    }

    protected override bool TryGetEntityInPool(IData<Projectile> data, out Projectile entity)
    {
        entity = EntityPool.First();
        entity.Initialize(data, this);
        return true;
    }
}
