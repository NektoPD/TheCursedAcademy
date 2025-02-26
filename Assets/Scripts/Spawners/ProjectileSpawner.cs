using Zenject;

public class ProjectileSpawner : Spawner<Projectile>
{
    [Inject]
    public void Construct(ProjectilePool pool)
    {
        _pool = pool;
    }
}
