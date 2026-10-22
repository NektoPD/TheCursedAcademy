using UnityEngine;

namespace Layers
{
    public class LayerController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;
    }
}
