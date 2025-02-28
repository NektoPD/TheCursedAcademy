using UnityEngine;

[RequireComponent(typeof(RangeAttacker))]
public class RangeEnemy : Enemy
{
    private RangeAttacker _attacker;

    private void Awake()
    {
        _attacker = GetComponent<RangeAttacker>();
        Mover = GetComponent<EnemyMover>();
        View = GetComponent<EnemyView>();
        DamageTaker = GetComponent<EnemyDamageTaker>();
    }

    public override void Initialize(IData<Enemy> data, EnemyPool pool)
    {
        RangeEnemyData enemyData = data as RangeEnemyData;

        base.Initialize(data, pool);
        _attacker.Initialize(enemyData.Cooldown, enemyData.Damage, enemyData.ProjectileData);
    }
}
