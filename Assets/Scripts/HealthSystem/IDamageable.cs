namespace HealthSystem
{
    public interface IDamageable
    {
        void TakeDamage(float damage, bool isFromBerserk = false);

        public bool IsDied { get; }
    }
}