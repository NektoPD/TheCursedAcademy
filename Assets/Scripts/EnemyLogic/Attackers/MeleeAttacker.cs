using Data.AttacksData;
using HealthSystem;
using UnityEngine;

namespace EnemyLogic.Attackers
{
    public class MeleeAttacker : Attacker
    {
        public override void Attack(AttackData data)
        {
            if (data is MeleeAttackData meleeData)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(EnemyAttacker.transform.position, meleeData.AttackRange);

                foreach (var hit in hits)
                    if (hit.TryGetComponent(out IDamageable damageable) && hit.TryGetComponent(out Enemy _) == false)
                        damageable.TakeDamage(meleeData.Damage);
            }
        }
    }
}
