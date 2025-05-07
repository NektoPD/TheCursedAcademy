using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class FadeController : MonoBehaviour
    {
        private const string FadeInTrigger = nameof(FadeInTrigger);

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void FadeIn() => _animator.SetTrigger(FadeInTrigger);
    }
}