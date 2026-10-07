using EnemyLogic.HealthBars;
using HealthSystem;
using System.Collections;
using CharacterLogic.Initializer;
using UnityEngine;
using Zenject;
using System;

namespace EnemyLogic
{
    [RequireComponent(typeof(HealthBar), typeof(EnemyAnimator), typeof(EnemyMover))]
    [RequireComponent(typeof(EnemyDamageView), typeof(EnemyEjector))]
    public class EnemyDamageTaker : MonoBehaviour, IDamageable
    {
        private readonly float _duration = 0.14f;

        private Health _health;
        private HealthBar _healthBar;
        private EnemyAnimator _enemyAnimator;
        private EnemyEjector _ejector;
        private EnemyMover _mover;
        private EnemyDamageView _damageView;
        private Coroutine _coroutine;
        private AudioSource _deathSound;
        private float _immuneTime;
        private Enemy _enemy;
        
        private bool _isDied = false;
        private bool _inImmune = false;
        private bool _wasKilledByBerserk;
        private CharacterInitializer _initializer;

        public event Action Died;

        public bool IsDied => _isDied;

        [Inject]
        public void Construct(AudioSource deathSound, CharacterInitializer initializer)
        {
            _deathSound = deathSound;
            _initializer = initializer;
        }
        
        private void Awake()
        {
            _healthBar = GetComponent<HealthBar>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _ejector = GetComponent<EnemyEjector>();
            _damageView = GetComponent<EnemyDamageView>();
            _mover = GetComponent<EnemyMover>();
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Died -= Die;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        public void Initialize(float maxHealth, float immuneTime, Enemy enemy)
        {
            _isDied = false;
            _inImmune = false;
            _enemyAnimator.SetDeadBool(false);
            _enemy = enemy;
            _wasKilledByBerserk = false;
            _enemyAnimator.ResetDeathState();

            _health = new Health(maxHealth);
            _healthBar.SetHealth(_health);
            _immuneTime = immuneTime;

            _health.Died += Die;
        }

        public float TakeDamage(float damage, bool isFromBerserk = false)
        {
            if (_isDied)
                return 0f;

            _wasKilledByBerserk = isFromBerserk;
            float appliedDamage = _health.TakeDamage(damage);
            if (appliedDamage <= 0f)
                return 0f;

            _damageView.ShowDamageNumber(appliedDamage);

            if (!_isDied && _inImmune == false)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);

                _coroutine = StartCoroutine(Countdown());
                _enemyAnimator.SetHurtTigger();
            }

            if (_initializer != null && _initializer.PlayerTransform != null)
                _damageView.StartFlash(_duration, _initializer.PlayerTransform.position);
            else
                _damageView.StartFlash(_duration);

            return appliedDamage;
        }

        private void Die()
        {
            if (_enemy != null)
                _enemy.DiedIventInvoke();
            else
                Died?.Invoke();

            _mover.Disable();
            _isDied = true;

            if (_deathSound != null)
                _deathSound.Play();

            _ejector.Eject();
            if (_wasKilledByBerserk)
                _enemyAnimator.PlayPopDeathAnimation();
            else
                _enemyAnimator.SetDeadBool(true);
        }

        private IEnumerator Countdown()
        {
            _inImmune = true;
            yield return new WaitForSeconds(_immuneTime);
            _inImmune = false;
        }
    }
}