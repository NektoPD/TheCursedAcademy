using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    private Vector3 _direction;
    private Transform _transform;
    private float _speed;

    private bool _canMove = true;

    private void OnEnable()
    {
        _transform = transform;
    }

    public void Initialize(Vector2 direction, float speed)
    {
        _canMove = true;
        _direction = direction;
        _speed = speed;
        _transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void ResetRotation() => _transform.rotation = Quaternion.identity;

    public void Disable() => _canMove = false;

    private void FixedUpdate()
    {
        if (_canMove)
            _transform.position += _speed * Time.fixedDeltaTime * _direction;
    }
}
