using UnityEngine;
using Zenject;

[RequireComponent (typeof(SpriteRenderer))]
public class LayerOrderController : MonoBehaviour
{
    [SerializeField] private int _upOrder = -1;
    [SerializeField] private int _downOrder = 1;

    private SpriteRenderer _spriteRenderer;
    private Transform _target;
    private float _offset = 0.2f;

    [Inject]
    public void Construct(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (transform.position.y < _target.position.y + _offset)
            _spriteRenderer.sortingOrder = _downOrder;
        else
            _spriteRenderer.sortingOrder = _upOrder;
    }
}
