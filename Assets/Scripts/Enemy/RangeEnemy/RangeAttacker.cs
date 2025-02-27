using UnityEngine;
using Zenject;

public class RangeAttacker : EnemyAttacker
{
    [SerializeField] private Transform _projectileSpawnPoint;

    private ProjectileData _projectileData;
    private ProjectilePool _projectilePool;

    [Inject]
    public void Construct(ProjectilePool projectilePool)
    {
        _projectilePool = projectilePool;
    }

    public void Initialize(float cooldowm, float damage, ProjectileData projectile)
    {
        Cooldown = cooldowm;
        Damage = damage;
        _projectileData = projectile;
    }

    protected override void AttackToggle()
    {
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        Projectile projectile = _projectilePool.Get(_projectileData);

        projectile.transform.position = _projectileSpawnPoint.position;

        projectile.SetDirection((Target.position - projectile.transform.position).normalized);
        projectile.SetDamage(Damage);
    }
}
