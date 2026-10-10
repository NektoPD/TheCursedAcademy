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
        private EnemyAttacker _attacker;
        private EnemyMover _mover;
        private Rigidbody2D _rb;
        private RigidbodyConstraints2D _constraintsBeforeFreeze;

        private Coroutine _immuneCoroutine;
        private bool _inImmune;
        private bool _isFrozen;

        private void Awake()
        {
            _animator = GetComponent<EnemyAnimator>();
            _view = GetComponent<EnemyFreezerView>();
            _attacker = GetComponent<EnemyAttacker>();
            _mover = GetComponent<EnemyMover>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Freeze()
        {
            if (_animator.IsDeadAnimationStarted || _inImmune || _isFrozen)
                return;

            _isFrozen = true;

            _animator.SetHurtTigger();

            _animator.SetAnimatorSpeed(0f);

            if (_rb != null)
            {
                _constraintsBeforeFreeze = _rb.constraints;
                _rb.velocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            _view.SetState(false);

            if (_attacker != null)
            {
                _attacker.StopAllCoroutines();
                _attacker.SetBlocked(true);
            }

            if (_immuneCoroutine != null) StopCoroutine(_immuneCoroutine);
            _immuneCoroutine = StartCoroutine(ImmuneCountdown());
        }

        public void Unfreeze()
        {
            if (_isFrozen == false)
                return;

            _isFrozen = false;

            if (_rb != null)
                _rb.constraints = _constraintsBeforeFreeze;

            _animator.ResetSpeed();
            _view.SetState(true);
            _attacker.SetBlocked(false);
        }

        private IEnumerator ImmuneCountdown()
        {
            _inImmune = true;
            yield return new WaitForSeconds(_immuneTimeInSecinds);
            _inImmune = false;
        }
    }
}