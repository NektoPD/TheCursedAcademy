using Zenject;

public class SpawnAttacker : Attacker
{
    private EnemyPool _pool;

    [Inject]
    public void Construct(EnemyPool pool)
    {
        _pool = pool;
    }

    public override void Attack(AttackData data)
    {
        if (data is SpawnAttackData spawnData)
        {
            for (int i = 0; i < EnemyAttacker.EnemySpawnPoints.Count; i++)
            {
                Enemy enemy = _pool.Get(spawnData.EnemysData[i % spawnData.EnemyCount]);
                enemy.transform.position = EnemyAttacker.EnemySpawnPoints[i].position;
            }
        }
    }
}
