using HealthSystem;
using UnityEngine;
using Zenject;

namespace EnemyLogic.HealthBars
{
    [RequireComponent(typeof(Enemy))]
    public class BossHealthBar : HealthBar
    {
        [SerializeField] private HealthPanel _healthPanelPrefab;

        private Transform _healthBarsContainer;
        private Enemy _enemy;
        private HealthPanel _healthPanel;

        [Inject]
        private void Construct(Transform healthBarContainer)
        {
            _healthBarsContainer = healthBarContainer;
        }

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _healthPanel = Instantiate(_healthPanelPrefab, _healthBarsContainer);
        }

        public override void SetHealth(Health health)
        {
            Bar = _healthPanel.Slider;
            _healthPanel.SetName(_enemy.Name);

            base.SetHealth(health);

            Health.Died += RemoveBar;
        }

        private void OnDisable()
        {
            if (Health != null)
                Health.Died -= RemoveBar;
        }

        private void RemoveBar() => Destroy(_healthPanel.gameObject);
    }
}