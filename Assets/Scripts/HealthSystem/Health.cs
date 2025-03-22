using System;

namespace HealthSystem
{
    public class Health
    {
        private float _currentHealth;

        public Health(float _maxHealth)
        {
            MaxHealth = _maxHealth;
            _currentHealth = _maxHealth;
        }

        public event Action Died;
        public event Action<float> Changed;

        public float MaxHealth { get; private set; }

        public void TakeHeal(float heal)
        {
            if (heal < 0)
                return;

            _currentHealth = Math.Clamp(_currentHealth + heal, 0, MaxHealth);
            Changed?.Invoke(_currentHealth);
        }

        public void TakeDamage(float damage)
        {
            if (damage < 0)
                return;

            _currentHealth = Math.Clamp(_currentHealth - damage, 0, MaxHealth);
            Changed?.Invoke(_currentHealth);

            if (_currentHealth == 0)
                Died?.Invoke();
        }
    }
}