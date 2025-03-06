using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private const string Speed = nameof(Speed);
    private const string Dead = nameof(Dead);
    private const string Hurt = nameof(Hurt);

    protected Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(RuntimeAnimatorController animatorController)
    {
        _animator.runtimeAnimatorController = animatorController;
    }

    public void SetSpeed(float speed) => _animator.SetFloat(Speed, speed);

    public void SetTriggerByName(string name) => _animator.SetTrigger(name);

    public void SetDeadTrigger() => SetTriggerByName(Dead);

    public void SetHurtTigger() => SetTriggerByName(Hurt);
}
