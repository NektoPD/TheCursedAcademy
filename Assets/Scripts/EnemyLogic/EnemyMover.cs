using CharacterLogic.Initializer;
using System;
using UnityEngine;
using Zenject;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyMover : MonoBehaviour
    {
        [SerializeField] private float _offsetDownY = 0.2f;
        [SerializeField] private float _offsetUpY = 0.2f;

        private readonly int _rotationAngle = 180;

        private CharacterInitializer _initializer;
        private Transform _transform;
        private float _speed;
        private float _attackRange;
        private EnemyAnimator _enemyView;
        private float _attackRangeSqr;
        private float _lastSpeed;
        
        private Vector2 _targetPosition;
        private bool _targetInRange;
        
        private bool _canMove = true;

        public event Action<Transform> TargetInRange;

        [Inject]
        public void Construct(CharacterInitializer initializer)
        {
            _initializer = initializer;
        }

        private void Awake()
        {
            _transform = transform;
            _enemyView = GetComponent<EnemyAnimator>();
        }
        
        private void Update()
        {
            if (!_canMove || _initializer == null)
                return;

            SetRotation(_initializer.PlayerTransform);

            Vector2 delta = GetCurrentPosition() - _initializer.PlayerTransform.position;
            _targetInRange = delta.sqrMagnitude <= _attackRangeSqr;
        }

        private void FixedUpdate()
        {
            if (!_canMove || _initializer == null)
                return;

            if (_targetInRange)
            {
                TargetInRange?.Invoke(_initializer.PlayerTransform);
                _enemyView.SetFloatSpeed(0);
                return;
            }

            _transform.position = Vector2.MoveTowards(
                _transform.position,
                _initializer.PlayerTransform.position,
                _speed * Time.fixedDeltaTime
            );

            _enemyView.SetFloatSpeed(_speed);
        }

        public void Initialize(float speed)
        {
            _canMove = true;
            _speed = speed;
        }

        public void SetAttackRange(float range)
        {
            _attackRange = range;
            _attackRangeSqr = range * range;
        }

        private void Disable() => _canMove = false;

        private void Enable() => _canMove = true;
        
        private void SetAnimatorSpeed(float speed)
        {
            if (Mathf.Approximately(_lastSpeed, speed))
                return;

            _lastSpeed = speed;
            _enemyView.SetFloatSpeed(speed);
        }

        private void SetRotation(Transform target)
        {
            if (target.position.x <= transform.position.x)
                _transform.localRotation = Quaternion.Euler(0, _rotationAngle, 0);
            else
                _transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        private Vector3 GetCurrentPosition()
        {
            Vector3 position = Vector3.zero;

            if (_initializer.PlayerTransform.position.y > _transform.position.y)
                position = new Vector3(_transform.position.x, _transform.position.y - _offsetDownY, _transform.position.z);
            else
                position = new Vector3(_transform.position.x, _transform.position.y - _offsetUpY, _transform.position.z);


            return position;
        }
    }
}