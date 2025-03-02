public abstract class Attacker
{
    protected EnemyAttacker EnemyAttacker;

    public abstract void Attack(AttackData data);

    public void SetAttacker(EnemyAttacker attacker) => EnemyAttacker = attacker;
}