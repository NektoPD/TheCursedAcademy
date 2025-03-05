using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyAnimator))]
public class EnemyMover : MonoBehaviour
{
    private readonly int _rotationAngle = 180;

    private Transform _target;
    private Transform _transform;
    private float _speed;
    private float _attackRange;
    private EnemyAnimator _enemyView;

    private bool _canMove = true;
    private float _offsetY = 0.7f;

    public event Action<Transform> TargetInRange;

    [Inject]
    public void Construct(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _transform = transform;
        _enemyView = GetComponent<EnemyAnimator>();
    }

    private void FixedUpdate()
    {
        if (_canMove == false)
        {
            _enemyView.SetSpeed(0);
            return;
        }

        SetRotation(_target);

        Vector3 position = GetCurrentPosition();

        if (Vector2.Distance(position, _target.transform.position) > _attackRange)
        {
            _transform.position = Vector2.MoveTowards(_transform.position, _target.position, _speed * Time.fixedDeltaTime);
            _enemyView.SetSpeed(_speed);
        }
        else
        {
            TargetInRange?.Invoke(_target);
            _enemyView.SetSpeed(0);
        }
    }

    public void Initialize(float speed)
    {
        _canMove = true;
        _speed = speed;
    }

    public void SetAttackRange(float range) => _attackRange = range;

    private void Disable() => _canMove = false;

    private void Enable() => _canMove = true;

    private void SetRotation(Transform target)
    {
        if (target.position.x <= transform.position.x)
            transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private Vector3 GetCurrentPosition()
    {
        Vector3 position = Vector3.zero;

        if (_target.position.y > _transform.position.y) 
            position = new Vector3(_transform.position.x, _transform.position.y - _offsetY, _transform.position.z);
        else
            position = new Vector3(_transform.position.x, _transform.position.y + _offsetY, _transform.position.z);

        return position;
    }
}
