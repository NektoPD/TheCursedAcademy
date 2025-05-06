using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimator : MonoBehaviour
    {
        private const string Speed = nameof(Speed);
        private const string Dead = nameof(Dead);
        private const string Hurt = nameof(Hurt);

        private float _speed;
        private Animator _animator;

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

        public void SetDeadBool(bool state)
        {
            if (_animator.GetBool(Dead) == state)
                return;

            _animator.SetBool(Dead, state);
        }

        public void SetAnimatorSpeed(float speed) => _animator.speed = speed;

        public void ResetSpeed() => _animator.speed = _speed;
    }
}
