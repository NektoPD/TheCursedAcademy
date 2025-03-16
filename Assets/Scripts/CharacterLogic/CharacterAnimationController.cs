using UnityEngine;

namespace CharacterLogic
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationController : MonoBehaviour
    {
        private const string WalkingKey = "IsWalking";
        private const string ProtectKey = "Protect";

        private readonly int _isWalking = Animator.StringToHash(WalkingKey);
        private readonly int _protecting = Animator.StringToHash(ProtectKey);
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetWalking(bool status)
        {
            _animator.SetBool(_isWalking, status);
        }

        public void SetProtecting()
        {
            _animator.SetTrigger(_protecting);
        }
        
        public void SetAnimatorOverride(AnimatorOverrideController overrideController)
        {
            _animator.runtimeAnimatorController = overrideController;
        }
    }
}