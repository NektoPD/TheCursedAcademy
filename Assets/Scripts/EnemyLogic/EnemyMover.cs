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

        private void FixedUpdate()
        {
            if (_canMove == false)
            {
                _enemyView.SetSpeed(0);
                return;
            }

            SetRotation(_initializer.PlayerTransform);

            if (Vector2.Distance(GetCurrentPosition(), _initializer.PlayerTransform.position) > _attackRange)
            {
                _transform.position = Vector2.MoveTowards(_transform.position, _initializer.PlayerTransform.position, _speed * Time.fixedDeltaTime);
                _enemyView.SetSpeed(_speed);
            }
            else
            {
                TargetInRange?.Invoke(_initializer.PlayerTransform);
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

            if (_initializer.PlayerTransform.position.y > _transform.position.y)
                position = new Vector3(_transform.position.x, _transform.position.y - _offsetDownY, _transform.position.z);
            else
                position = new Vector3(_transform.position.x, _transform.position.y - _offsetUpY, _transform.position.z);


            return position;
        }
    }
}