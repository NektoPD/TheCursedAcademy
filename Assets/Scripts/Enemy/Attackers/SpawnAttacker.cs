using UnityEngine;
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
            for (int i = 0; i < spawnData.EnemyCount; i++)
            {
                Enemy enemy = _pool.Get(spawnData.EnemysData[Random.Range(0, spawnData.EnemysData.Count)]);
                enemy.transform.position = EnemyAttacker.EnemySpawnPoints[i % EnemyAttacker.EnemySpawnPoints.Count].position;
            }
        }
    }
}
