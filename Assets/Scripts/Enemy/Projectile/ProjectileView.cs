using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class ProjectileView : MonoBehaviour
{
    private SpriteRenderer _renderer;

    private void OnEnable()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }
}
