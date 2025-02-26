using System;
using CharacterLogic.InputHandler;
using UnityEngine;

namespace CharacterLogic
{
    [RequireComponent(typeof(CharacterAnimationController))]
    [RequireComponent(typeof(CharacterMovementHandler))]
    [RequireComponent(typeof(Health))]
    public class CharacterController : MonoBehaviour
    {
        private CharacterData _characterData;
        private CharacterAnimationController _animationController;
        private CharacterMovementHandler _movementHandler;
        private Health _health;
        private CharacterSpriteHolder _spriteHolder;

        private void Awake()
        {
            _animationController = GetComponent<CharacterAnimationController>();
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _health = GetComponent<Health>();
            _spriteHolder = GetComponent<CharacterSpriteHolder>();
        }

        public void Construct(CharacterData data)
        {
            _characterData = data;
        }

    }
}