using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyView : MonoBehaviour
{
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
}
