using CharacterLogic.Initializer;
using Data.AttacksData;
using EnemyLogic.ProjectileLogic;
using Pools;
using Zenject;

namespace EnemyLogic.Attackers
{
    public class RangeAttacker : Attacker
    {
        private ProjectilePool _pool;
        private CharacterInitializer _initializer;

        [Inject]
        public void Construct(ProjectilePool pool, CharacterInitializer initializer)
        {
            _pool = pool;
            _initializer = initializer;
        }

        public override void Attack(AttackData data)
        {
            if (data is RangeAttackData rangeData)
            {
                for (int i = 0; i < rangeData.CountProjectiles; i++)
                {
                    Projectile projectile = _pool.Get(rangeData.ProjectileData);

                    var sp = EnemyAttacker.ProjectileSpawnPoints[i % EnemyAttacker.ProjectileSpawnPoints.Count];
                    
                    projectile.transform.position = sp.position;

                    projectile.SetDamage(rangeData.Damage);
                    projectile.SetDirection((_initializer.PlayerTransform.position - EnemyAttacker.ProjectileSpawnPoints[i].position).normalized);
                }
            }
        }
    }
}
