namespace HealthSystem
{
    public interface IDamageable
    {
        void TakeDamage(float damage);

        public bool IsDied { get; }
    }
}