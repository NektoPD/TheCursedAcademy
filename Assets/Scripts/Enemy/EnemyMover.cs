using System;
using UnityEngine;
using Zenject;

public class EnemyMover : MonoBehaviour
{
    private readonly int _rotationAngle = 180;

    private Transform _target;
    private Transform _transform;
    private float _speed;
    private float _attackRange;
    public event Action<Transform> TargetInRange;

    [Inject]
    public void Construct(Transform target)
    {
        _target = target;
    }

    private void Awake()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        SetRotation(_target);

        if (Vector2.Distance(transform.position, _target.transform.position) > _attackRange)
            _transform.position = Vector2.MoveTowards(_transform.position, _target.position, _speed * Time.fixedDeltaTime);
        else
            TargetInRange?.Invoke(_target);
    }

    public void Initialize(float speed, float attackRange)
    {
        _speed = speed;
        _attackRange = attackRange;
    }

    private void SetRotation(Transform target)
    {
        if (target.position.x <= transform.position.x)
            transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
