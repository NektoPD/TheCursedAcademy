using System;

namespace HealthSystem
{
    public class Health
    {
        private readonly float _lowHealthThreshold = 20f;
        private float _currentHealth;

        public event Action LowHealth;
        public event Action HealthRegainedToNormal;

        public Health(float maxHealth)
        {
            SetMaxHealth(maxHealth);
        }

        public event Action Died;
        public event Action<float> Changed;

        public float MaxHealth { get; private set; }

        public void SetMaxHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeHeal(float heal)
        {
            if (heal < 0)
                return;

            _currentHealth = Math.Clamp(_currentHealth + heal, 0, MaxHealth);
            Changed?.Invoke(_currentHealth);

            if (_currentHealth >= _lowHealthThreshold)
                HealthRegainedToNormal?.Invoke();
        }

        public void TakeDamage(float damage)
        {
            if (damage < 0)
                return;

            _currentHealth = Math.Clamp(_currentHealth - damage, 0, MaxHealth);
            Changed?.Invoke(_currentHealth);

            if (_currentHealth <= _lowHealthThreshold)
                LowHealth?.Invoke();

            if (_currentHealth == 0)
                Died?.Invoke();
        }
    }
}