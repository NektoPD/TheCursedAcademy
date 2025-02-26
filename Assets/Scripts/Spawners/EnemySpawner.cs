using Zenject;

public class EnemySpawner : Spawner<Enemy>
{
    [Inject]
    public void Construct(EnemyPool pool)
    {
        _pool = pool;
    }
}
