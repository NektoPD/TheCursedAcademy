using UnityEngine;

[RequireComponent (typeof(SpriteRenderer), typeof(Animator), typeof(ResizeCollider))]
public class ProjectileView : MonoBehaviour
{
    private const string Hit = nameof(Hit);

    private SpriteRenderer _renderer;
    private Animator _animator;
    private ResizeCollider _resizeCollider;

    private void OnEnable()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _resizeCollider = GetComponent<ResizeCollider>();
    }

    public void Initialize(Sprite sprite, RuntimeAnimatorController animator)
    {
        _renderer.sprite = sprite;
        _animator.runtimeAnimatorController = animator;
        _resizeCollider.Resize();
    }

    public void SetHitTrigger() => _animator.SetTrigger(Hit);
}
