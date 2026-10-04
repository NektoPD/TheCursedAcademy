using CharacterLogic;
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
                Vector2 attackCenter = EnemyAttacker.transform.position;
                foreach (Collider2D collider in EnemyAttacker.GetComponentsInChildren<Collider2D>())
                {
                    if (collider.isTrigger)
                        continue;

                    attackCenter = collider.bounds.center;
                    break;
                }

                Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, meleeData.AttackRange);

                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<CharacterCollisionHandler>(out _))
                        continue;

                    if (hit.GetComponentInParent<Enemy>() != null)
                        continue;

                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    damageable ??= hit.GetComponentInParent<Character>();

                    damageable?.TakeDamage(meleeData.Damage);
                }
            }
        }
    }
}
