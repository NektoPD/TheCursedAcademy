using EnemyLogic.HealthBars;
using HealthSystem;
using System.Collections;
using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(HealthBar), typeof(EnemyAnimator))]
    [RequireComponent (typeof(EnemyDamageView), typeof(EnemyEjector))]
    public class EnemyDamageTaker : MonoBehaviour, IDamageable
    {
        private readonly int _duration = 1;

        private Health _health;
        private HealthBar _healthBar;
        private EnemyAnimator _enemyAnimator;
        private EnemyEjector _ejector;
        private EnemyDamageView _damageView;
        private Coroutine _coroutine;
        private float _immuneTime;

        private bool _isDied = false;
        private bool _inImmune = false;

        private void Awake()
        {
            _healthBar = GetComponent<HealthBar>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _ejector = GetComponent<EnemyEjector>();
            _damageView = GetComponent<EnemyDamageView>();
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Died -= Die;

            if(_coroutine != null)
                StopCoroutine(_coroutine);
        }

        public void Initialize(float maxHealth, float immuneTime)
        {
            _isDied = false;
            _health = new Health(maxHealth);
            _healthBar.SetHealth(_health);
            _immuneTime = immuneTime;

            _health.Died += Die;
        }

        public void TakeDamage(float damage)
        {
            if (_isDied)
                return;

            _health.TakeDamage(damage);

            if (_inImmune == false)
            {
                _coroutine = StartCoroutine(Countdown());
                _enemyAnimator.SetHurtTigger();
            }

            _damageView.StartFlash(_duration);
        }

        private void Die()
        {
            _isDied = true;
            _ejector.Eject();
            _enemyAnimator.SetDeadTrigger();
        }

        private IEnumerator Countdown()
        {
            _inImmune = true;
            yield return new WaitForSeconds(_immuneTime);
            _inImmune = false;
        }
    }
}