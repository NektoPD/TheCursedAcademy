using System;
using System.Collections.Generic;

public class AttackerManager
{
    private readonly Dictionary<Type, Attacker> _attackers;

    public AttackerManager(MeleeAttacker meleeAttacker, RangeAttacker rangedAttacker, SpawnAttacker spawnAttacker)
    {
        _attackers = new Dictionary<Type, Attacker>
        {
            { typeof(MeleeAttackData), meleeAttacker },
            { typeof(RangeAttackData), rangedAttacker },
            { typeof(SpawnAttackData), spawnAttacker }
        };
    }

    public void ExecuteAttack(AttackData data, EnemyAttacker enemyAttacker)
    {
        if (_attackers.TryGetValue(data.GetType(), out var attacker))
        {
            attacker.SetAttacker(enemyAttacker);
            attacker.Attack(data);
        }
    }
}
