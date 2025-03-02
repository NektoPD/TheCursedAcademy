using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;

    private const string Speed = nameof(Speed);
    private const string Dead = nameof(Dead);
    private const string Hurt = nameof(Hurt);

    private Health _health;
    protected Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        _health.Changed -= OnHealthChanged;
    }

    public void Initialize(RuntimeAnimatorController animatorController)
    {
        _animator.runtimeAnimatorController = animatorController;
    }

    public void SetHealth(Health health)
    {
        _health = health;

        _hpBar.maxValue = _health.MaxHealth;
        _hpBar.value = _health.MaxHealth;

        health.Changed += OnHealthChanged;
    }

    public void SetSpeed(float speed) => _animator.SetFloat(Speed, speed);

    public void SetTriggerByName(string name) => _animator.SetTrigger(name);

    public void SetDeadTrigger() => SetTriggerByName(Dead);

    public void SetHurtTigger() => SetTriggerByName(Hurt);

    private void OnHealthChanged(float health) => _hpBar.value = health;
}
