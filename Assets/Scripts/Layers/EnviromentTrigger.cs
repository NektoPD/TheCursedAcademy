using System.Linq;
using UnityEngine;

namespace Layers
{
    [RequireComponent(typeof(Collider2D))]
    public class EnviromentTrigger : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _renderers;
        [SerializeField] private float _transparencyAmount = 0.5f;

        private Color _originalColor;
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            _originalColor = _renderers.First().color;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_collider.bounds.min.y > collision.bounds.min.y)
                return;

            foreach (var renderer in _renderers)
                renderer.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _transparencyAmount);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            foreach (var renderer in _renderers)
                renderer.color = _originalColor;
        }
    }
}