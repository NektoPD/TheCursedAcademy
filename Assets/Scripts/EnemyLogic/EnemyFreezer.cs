using Items.Interfaces;
using System.Collections;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyAnimator), typeof(EnemyFreezerView))]
    public class EnemyFreezer : MonoBehaviour, IFreezable
    {
        [SerializeField] private float _immuneTimeInSecinds;

        private EnemyAnimator _animator;
        private EnemyFreezerView _view;
        private Coroutine _coroutine;
        private bool _inImmune = false;

        private void Awake()
        {
            _animator = GetComponent<EnemyAnimator>();
            _view = GetComponent<EnemyFreezerView>();
        }

        private void OnEnable()
        {
            _animator.DeadAnimationStarted += Unfreeze;
            _inImmune = false;
        }

        private void OnDisable()
        {
            _animator.DeadAnimationStarted -= Unfreeze;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        public void Freeze()
        {
            if (_animator.IsDeadAnimationStarted || _inImmune)
                return;

            _animator.SetHurtTigger();
            _animator.SetAnimatorSpeed(0f);
            _view.SetState(false);

            _coroutine = StartCoroutine(Countdown());
        }

        public void Unfreeze()
        {
            _animator.ResetSpeed();
            _view.SetState(true);
        }

        private IEnumerator Countdown()
        {
            _inImmune = true;
            yield return new WaitForSeconds(_immuneTimeInSecinds);
            _inImmune = false;
        }
    }
}