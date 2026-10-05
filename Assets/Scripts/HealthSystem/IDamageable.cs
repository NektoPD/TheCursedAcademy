namespace HealthSystem
{
    public interface IDamageable
    {
        float TakeDamage(float damage, bool isFromBerserk = false);

        public bool IsDied { get; }
    }
}