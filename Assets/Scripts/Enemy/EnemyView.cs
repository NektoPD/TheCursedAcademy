using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;

    private const string Speed = nameof(Speed);
    private const string Dead = nameof(Dead);
    private const string Attack = nameof(Attack);
    private const string Hurt = nameof(Hurt);

    private SpriteRenderer _renderer;
    private Animator _animator;
    private Health _health;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        _health.Changed -= OnHealthChanged;
    }

    public void Initialize(Sprite sprite, RuntimeAnimatorController animatorController)
    {
        _renderer.sprite = sprite;
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

    public void SetAttackTrigger() => _animator.SetTrigger(Attack);

    public void SetDeadTrigger() => _animator.SetTrigger(Dead);

    public void SetHurtTigger() => _animator.SetTrigger(Hurt);


    private void OnHealthChanged(float health) => _hpBar.value = health;
}
