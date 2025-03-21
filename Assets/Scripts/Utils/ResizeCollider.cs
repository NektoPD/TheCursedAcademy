using UnityEngine;

namespace Utils
{
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

        public void Resize()
        {
            _collider.size = _renderer.sprite.bounds.size;
        }
    }
}