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
        public event Action MovingUp;
        public event Action MovingDown;
        public event Action MovingDiagonally;

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
            CheckMovementEvents();
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
                return;

            float scaledSpeed = _moveSpeed * Time.deltaTime;
            Vector2 offset = new Vector2(_moveDirection.x, _moveDirection.y) * scaledSpeed;

            _transform.Translate(offset);
        }

        private void CheckMovementEvents()
        {
            if (_moveDirection.x < 0)
                MovingLeft?.Invoke();
            else if (_moveDirection.x > 0)
                MovingRight?.Invoke();

            if (_moveDirection.y > 0)
                MovingUp?.Invoke();
            else if (_moveDirection.y < 0)
                MovingDown?.Invoke();

            if (Mathf.Abs(_moveDirection.x) > 0.1f && Mathf.Abs(_moveDirection.y) > 0.1f)
                MovingDiagonally?.Invoke();
        }

        public bool IsMoving()
        {
            return _moveDirection.sqrMagnitude > 0.1f;
        }

        public bool IsMovingLeft()
        {
            return _moveDirection.x < 0;
        }

        public bool IsMovingRight()
        {
            return _moveDirection.x > 0;
        }

        public bool IsMovingUp()
        {
            return _moveDirection.y > 0;
        }

        public bool IsMovingDown()
        {
            return _moveDirection.y < 0;
        }

        public bool IsMovingDiagonally()
        {
            return Mathf.Abs(_moveDirection.x) > 0.1f && Mathf.Abs(_moveDirection.y) > 0.1f;
        }

        public Vector2 GetMoveDirection()
        {
            return _moveDirection.normalized;
        }

        public float GetMoveAngle()
        {
            if (_moveDirection.sqrMagnitude < 0.1f)
                return 0f;

            return Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
        }
    }
}