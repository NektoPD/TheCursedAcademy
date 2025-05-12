using Items.Interfaces;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EnemyMover), typeof(EnemyAttacker), typeof(EnemyAnimator))]
    public class EnemyFreezer : MonoBehaviour, IFreezable
    {
        [SerializeField] private float _duration = 2f;
        [SerializeField] private Color _color = Color.blue;

        private SpriteRenderer _spriteRenderer;
        private EnemyMover _mover;
        private EnemyAttacker _attacker;
        private EnemyAnimator _animator;
        private Color _defaultColor;
        private Coroutine _coroutine;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<EnemyAnimator>();
            _attacker = GetComponent<EnemyAttacker>();
            _mover = GetComponent<EnemyMover>();
        }

        private void Start()
        {
            _defaultColor = _spriteRenderer.color;
        }

        private void OnEnable()
        {
            _animator.DeadAnimationStarted += Unfreeze;
        }

        private void OnDisable()
        {
            _animator.DeadAnimationStarted -= Unfreeze;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _spriteRenderer.color = _defaultColor;
        }

        public void Freeze()
        {
            if (_animator.IsDeadAnimationStarted)
                return;

            _animator.SetHurtTigger();
            _animator.SetAnimatorSpeed(0f);
            SetState(false, _defaultColor, _color);
        }

        public void Unfreeze()
        {
            _animator.ResetSpeed();
            SetState(true, _color, _defaultColor);
        }

        private void SetState(bool componentEnabled, Color firstColor, Color secondColor)
        {
            if (gameObject.activeSelf == false)
                return;

            _mover.enabled = componentEnabled;
            _attacker.enabled = componentEnabled;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(ChangeColor(firstColor, secondColor));
        }

        private IEnumerator ChangeColor(Color startColor, Color endColor)
        {
            float timeElapsed = 0f;

            while (timeElapsed < _duration)
            {
                _spriteRenderer.color = Color.Lerp(startColor, endColor, timeElapsed / _duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.color = endColor;
        }
    }
}