using Zenject;

public class ProjectilePool : Pool<Projectile>
{
    public ProjectilePool(DiContainer container) : base(container) { }

    protected override Projectile Create(IData<Projectile> data)
    {
        Projectile newProjectile = _container.InstantiatePrefabForComponent<Projectile>(data.Prefab);
        newProjectile.Initialize(data, this);
        return newProjectile;
    }

    protected override Projectile GetInitializedEntity(IData<Projectile> data, Projectile entity)
    {
        entity.Initialize(data, this);
        return entity;
    }
}
