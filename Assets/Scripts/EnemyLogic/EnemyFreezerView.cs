using System.Collections;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyMover), typeof(EnemyAttacker), typeof(EnemyDamageView))]
    public class EnemyFreezerView : MonoBehaviour
    {
        [SerializeField] private float _duration = 2f;
        [SerializeField] private Color _color = Color.blue;

        private SpriteRenderer _spriteRenderer;
        private EnemyMover _mover;
        private EnemyAttacker _attacker;
        private EnemyDamageView _damageView;
        private Color _defaultColor;
        private Coroutine _coroutine;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _damageView = GetComponent<EnemyDamageView>();
            _attacker = GetComponent<EnemyAttacker>();
            _mover = GetComponent<EnemyMover>();
        }

        private void OnEnable()
        {
            _defaultColor = _spriteRenderer.color;
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _spriteRenderer.color = _defaultColor;
        }

        public void SetState(bool componentEnabled)
        {
            if (gameObject.activeSelf == false)
                return;

            _mover.enabled = componentEnabled;
            _attacker.enabled = componentEnabled;
            _damageView.enabled = componentEnabled;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            if(componentEnabled == true)
                _coroutine = StartCoroutine(ChangeColor(_defaultColor, _color));
            else
                _coroutine = StartCoroutine(ChangeColor(_color, _defaultColor));
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