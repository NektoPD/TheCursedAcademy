using UnityEngine;
using Zenject;

[RequireComponent(typeof(Enemy))]
public class BossHealthBar : HealthBar
{
    [SerializeField] private HealthPanel _healthPanelPrefab;

    private Transform _healthBarsContainer;
    private Enemy _enemy;

    [Inject]
    private void Construct(Transform healthBarContainer)
    {
        _healthBarsContainer = healthBarContainer;
    }

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    public override void SetHealth(Health health)
    {
        base.SetHealth(health);

        var healthPanel = Instantiate(_healthPanelPrefab, _healthBarsContainer);

        Bar = healthPanel.Slider;
        healthPanel.SetName(_enemy.Name);
    }

    private void OnEnable()
    {
        Health.Died += RemoveBar;
    }

    private void RemoveBar() => Destroy(Bar);
}
