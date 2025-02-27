using UnityEngine;

[RequireComponent (typeof(MeleeAttacker))]
public class MeleeEnemy : Enemy
{
    private MeleeAttacker _attacker;

    private void Awake()
    {
        _attacker = GetComponent<MeleeAttacker>();
        Mover = GetComponent<EnemyMover>();
        View = GetComponent<EnemyView>();
        DamageTaker = GetComponent<EnemyDamageTaker>();
    }

    public override void Initialize(IData<Enemy> data, EnemyPool pool)
    {
        MeleeEnemyData enemyData = data as MeleeEnemyData;

        base.Initialize(enemyData, pool);
        _attacker.Initialize(enemyData.Cooldown, enemyData.AttackRange, enemyData.Damage);
    }
}
