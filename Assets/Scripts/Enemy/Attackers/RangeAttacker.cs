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
            for (int i = 0; i < rangeData.CountProjectiles; i++)
            {
                Projectile projectile = _pool.Get(rangeData.ProjectileData);

                projectile.transform.position = EnemyAttacker.ProjectileSpawnPoints[i].position;

                projectile.SetDirection((_target.position - EnemyAttacker.ProjectileSpawnPoints[i].position).normalized);
                projectile.SetDamage(rangeData.Damage);
            }
        }
    }
}
