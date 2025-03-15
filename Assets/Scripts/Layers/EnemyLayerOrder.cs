using UnityEngine;
using Zenject;

[RequireComponent (typeof(LayerController))]
public class EnemyLayerOrder : MonoBehaviour
{
    [SerializeField] private int _upOrder = -1;
    [SerializeField] private int _downOrder = 2;

    private LayerController _layerController;
    private Transform _target;

    private readonly float _offset = 0.2f;

    [Inject]
    public void Construct(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _layerController = GetComponent<LayerController>();
    }

    private void Update()
    {
        if (transform.position.y < _target.position.y + _offset)
            _layerController.SetSortingOrder(_downOrder);
        else
            _layerController.SetSortingOrder(_upOrder);
    }
}
