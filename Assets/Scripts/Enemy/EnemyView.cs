using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    private const string Speed = nameof(Speed);
    private const string Dead = nameof(Dead);
    private const string Attack = nameof(Attack);
    private const string Hurt = nameof(Hurt);

    private SpriteRenderer _renderer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Sprite sprite, RuntimeAnimatorController animatorController)
    {
        _renderer.sprite = sprite;
        _animator.runtimeAnimatorController = animatorController;
    }

    public void SetSpeed(float speed) => _animator.SetFloat(Speed, speed);

    public void SetAttackTrigger() => _animator.SetTrigger(Attack);

    public void SetDeadTrigger() => _animator.SetTrigger(Dead);

    public void SetHurtTigger() => _animator.SetTrigger(Hurt);
}
