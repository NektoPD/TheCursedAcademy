using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class ResizeCollider : MonoBehaviour
{
    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _collider.size = _renderer.bounds.size;
    }
}
