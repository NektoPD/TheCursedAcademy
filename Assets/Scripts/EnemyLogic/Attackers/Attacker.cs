using Data.AttacksData;

namespace EnemyLogic.Attackers
{
    public abstract class Attacker
    {
        protected EnemyAttacker EnemyAttacker;

        public abstract void Attack(AttackData data);

        public void SetBaseAttacker(EnemyAttacker attacker) => EnemyAttacker = attacker;
    }
}