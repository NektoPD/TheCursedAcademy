using System;
using UnityEngine;

namespace CharacterLogic.InputHandler
{
    public class CharacterMovementHandler : MonoBehaviour
    {
        private float _moveSpeed;
        private Vector2 _moveDirection;
        private PlayerInput _playerInput;
        private Transform _transform;

        public event Action MovingLeft;
        public event Action MovingRight;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _transform = transform;
        }

        private void OnEnable()
        {
            EnableMovement();
        }

        private void OnDisable()
        {
            DisableMovement();
        }

        private void Update()
        {
            _moveDirection = _playerInput.Player.Move.ReadValue<Vector2>();
            Move();
        }

        public void SetSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        public void EnableMovement()
        {
            _playerInput.Enable();
        }

        public void DisableMovement()
        {
            _playerInput.Disable();
        }

        private void Move()
        {
            if (_moveDirection.sqrMagnitude < 0.1f)
            {
                Debug.Log("not moving");
                return;
            }

            Debug.Log("moving");

            float scaledSpeed = _moveSpeed * Time.deltaTime;
            Vector2 offset = new Vector2(_moveDirection.x, _moveDirection.y) * scaledSpeed;

            _transform.Translate(offset);

            if (_moveDirection.x < 0)
                MovingLeft?.Invoke();
            else if (_moveDirection.x > 0)
                MovingRight?.Invoke();
        }

        public bool IsMoving()
        {
            return _moveDirection.sqrMagnitude > 0.1f;
        }

        public bool IsMovingLeft()
        {
            return _moveDirection.x < 0;
        }
    }
}