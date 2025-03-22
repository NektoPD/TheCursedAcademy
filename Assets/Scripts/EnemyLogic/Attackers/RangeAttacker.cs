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

                    projectile.transform.position = EnemyAttacker.ProjectileSpawnPoints[i].position;

                    projectile.SetDirection((_initializer.PlayerTransform.position - EnemyAttacker.ProjectileSpawnPoints[i].position).normalized);
                    projectile.SetDamage(rangeData.Damage);
                }
            }
        }
    }
}
