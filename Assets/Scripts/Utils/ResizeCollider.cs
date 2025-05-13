using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class ResizeCollider : MonoBehaviour
    {
        [SerializeField] private float _offset = 0.1f;

        private BoxCollider2D _collider;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Resize()
        {
            var size = new Vector3(_renderer.sprite.bounds.size.x - _offset, _renderer.sprite.bounds.size.y - _offset, _renderer.sprite.bounds.size.z - _offset);
            _collider.size = size;
        }
    }
}