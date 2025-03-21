using UnityEngine;

namespace Layers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LayerController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;
    }
}
