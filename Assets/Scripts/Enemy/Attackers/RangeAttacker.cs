using UnityEngine;
using Zenject;

public class RangeAttacker : Attacker
{
    private ProjectilePool _pool;
    private Transform _target;

    [Inject]
    public void Construct(ProjectilePool pool, Transform target)
    {
        _pool = pool;
        _target = target;
    }

    public override void Attack(AttackData data)
    {
        if (data is RangeAttackData rangeData)
        {
            Projectile projectile = _pool.Get(rangeData.ProjectileData);

            projectile.transform.position = EnemyAttacker.CurrentProjectileSpawnPosition;

            projectile.SetDirection((_target.position - EnemyAttacker.CurrentProjectileSpawnPosition).normalized);
            projectile.SetDamage(rangeData.Damage);
        }
    }
}
