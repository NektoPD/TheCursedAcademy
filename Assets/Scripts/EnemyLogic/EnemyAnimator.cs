using System;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimator : MonoBehaviour
    {
        private const string Speed = nameof(Speed);
        private const string Dead = nameof(Dead);
        private const string Hurt = nameof(Hurt);
        private const string Pop = nameof(Pop);

        private float _speed;
        private Animator _animator;

        public bool IsDeadAnimationStarted = false;

        public event Action DeadAnimationStarted;

        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(RuntimeAnimatorController animatorController)
        {
            _animator.runtimeAnimatorController = animatorController;
            _speed = _animator.speed;
        }

        public void SetFloatSpeed(float speed) => _animator.SetFloat(Speed, speed);

        public void SetTriggerByName(string name) => _animator.SetTrigger(name);

        public void SetHurtTigger() => SetTriggerByName(Hurt);

        public void SetDeadBool(bool state) => SetBoolAsDead(state, Dead);

        public void ResetDeathState()
        {
            _animator.SetBool(Dead, false);
            _animator.SetBool(Pop, false);
            IsDeadAnimationStarted = false;
        }

        public void SetPopBool(bool state) => SetBoolAsDead(state, Pop);

        public void SetAnimatorSpeed(float speed) => _animator.speed = speed;

        public void ResetSpeed() => _animator.speed = _speed;

        private void SetBoolAsDead(bool state, string name)
        {
            if (_animator.GetBool(name) == state)
                return;

            _animator.SetBool(name, state);

            IsDeadAnimationStarted = true;

            DeadAnimationStarted?.Invoke();
        }
    }
}
