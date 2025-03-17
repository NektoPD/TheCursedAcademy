using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class LayerTrigger : MonoBehaviour
{
    [SerializeField] private int _upOrder = -1;
    [SerializeField] private int _downOrder = 2;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LayerController controller))
        {
            if (_collider.bounds.min.y > collision.bounds.min.y)
                controller.SetSortingOrder(_downOrder);
            else
                controller.SetSortingOrder(_upOrder);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LayerController controller))
            controller.SetSortingOrder(_downOrder);
    }
}
