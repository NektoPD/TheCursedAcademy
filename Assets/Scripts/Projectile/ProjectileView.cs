using UnityEngine;

[RequireComponent (typeof(SpriteRenderer), typeof(Animator))]
public class ProjectileView : MonoBehaviour
{
    private const string Hit = nameof(Hit);

    private SpriteRenderer _renderer;
    private Animator _animator;

    private void OnEnable()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void Initialize(Sprite sprite, RuntimeAnimatorController animator)
    {
        _renderer.sprite = sprite;
        _animator.runtimeAnimatorController = animator;
    }

    public void SetHitTrigger() => _animator.SetTrigger(Hit);
}
