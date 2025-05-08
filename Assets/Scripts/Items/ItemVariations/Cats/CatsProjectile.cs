using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using Items.ItemVariations;
using UnityEngine;
using UnityEngine.Pool;

namespace Items.ItemVariations.Cats
{
    public class CatsProjectile : ItemProjectile
    {
        private const string WalkParam = "IsWalking";
        private const string RunParam = "IsRunning";
        private const string MeowParam = "IsMeowing";
        private const string IdleParam = "IsIdle";

        private enum CatState
        {
            Walking,
            Running,
            Attacking,
            Idle
        }

        [Header("Animation")] [SerializeField] private Animator _animator;

        [Header("Combat Settings")] [SerializeField]
        private float _attackCooldown = 0.5f;

        [SerializeField] private float _optimalAttackDistance = 1.5f;
        [SerializeField] private float _attackDistanceThreshold = 0.3f;
        [SerializeField] private LayerMask _enemyLayer;

        [Header("Behavior Settings")] [SerializeField]
        private float _idleChance = 0.1f;

        [SerializeField] private float _idleDuration = 2f;
        [SerializeField] private float _stateCheckInterval = 0.2f;
        [SerializeField] private float _idleCheckTime = 3f;
        [SerializeField] private float _minDistanceToPlayer = 1.0f;
        [SerializeField] private float _maxDistanceFromPlayer = 3f;
        [SerializeField] private float _walkSpeedMultiplier = 0.7f;
        [SerializeField] private float _nearPlayerSpeedMultiplier = 0.3f;
        [SerializeField] private float _returnToPlayerSpeedMultiplier = 0.5f;
        [SerializeField] private float _positionMaintenanceSpeedMultiplier = 0.5f;
        [SerializeField] private float _walkFrequencyX = 1.5f;
        [SerializeField] private float _walkFrequencyY = 1.2f;

        [Header("Effects")] [SerializeField] private ParticleSystem _disappearEffectPrefab;

        private IObjectPool<CatsProjectile> _pool;
        private Transform _playerTransform;
        private float _speed;
        private float _detectionRadius;
        private Coroutine _lifetimeCoroutine;
        private Coroutine _stateCoroutine;
        private CatState _currentState = CatState.Walking;
        private Transform _targetEnemy;
        private float _timeSinceLastStateChange;
        private ParticleSystem _disappearEffect;
        private CatsItem _catsController;

        public void SetPool(IObjectPool<CatsProjectile> pool)
        {
            _pool = pool;
        }

        public void SetController(CatsItem controller)
        {
            _catsController = controller;
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeParticleEffect();
        }

        private void InitializeParticleEffect()
        {
            if (_disappearEffectPrefab != null && _disappearEffect == null)
            {
                _disappearEffect = Instantiate(_disappearEffectPrefab);
                _disappearEffect.gameObject.SetActive(false);
            }
        }

        public void Activate(float speed, float lifetime, float detectionRadius, Transform playerTransform)
        {
            _speed = speed;
            _detectionRadius = detectionRadius;
            _playerTransform = playerTransform;
            _currentState = CatState.Walking;
            UpdateAnimatorState();

            _targetEnemy = null;

            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
            }

            if (_stateCoroutine != null)
            {
                StopCoroutine(_stateCoroutine);
            }

            _lifetimeCoroutine = StartCoroutine(ReturnToPoolAfterTime(lifetime));
            _stateCoroutine = StartCoroutine(StateMachine());
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            _timeSinceLastStateChange += Time.deltaTime;

            switch (_currentState)
            {
                case CatState.Walking:
                    WalkAroundPlayer();
                    break;
                case CatState.Running:
                    RunTowardsEnemy();
                    break;
                case CatState.Attacking:
                    MaintainAttackPosition();
                    break;
                case CatState.Idle:
                    break;
            }
        }

        private IEnumerator StateMachine()
        {
            while (true)
            {
                if (_currentState != CatState.Attacking)
                {
                    CheckForEnemies();
                }

                if (_currentState == CatState.Walking && _timeSinceLastStateChange > _idleCheckTime &&
                    Random.value < _idleChance)
                {
                    ChangeState(CatState.Idle);
                    yield return new WaitForSeconds(_idleDuration);
                    ChangeState(CatState.Walking);
                }

                if ((_currentState == CatState.Running || _currentState == CatState.Attacking) &&
                    (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy))
                {
                    if (_targetEnemy != null && _targetEnemy.TryGetComponent(out IDamageable targetDamageable))
                    {
                        if (_catsController != null)
                        {
                            _catsController.ReleaseEnemyTarget(targetDamageable);
                        }
                    }

                    _targetEnemy = null;
                    ChangeState(CatState.Walking);
                }

                yield return new WaitForSeconds(_stateCheckInterval);
            }
        }

        private void CheckForEnemies()
        {
            if (_catsController == null) return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(Transform.position, _detectionRadius, _enemyLayer);

            if (colliders.Length > 0)
            {
                float closestDistance = float.MaxValue;
                Transform closestEnemy = null;
                IDamageable closestDamageable = null;

                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out IDamageable damageable) &&
                        !collider.TryGetComponent(out CharacterLogic.Character character))
                    {
                        if (_catsController.IsEnemyTargeted(damageable) && _targetEnemy == null)
                        {
                            continue;
                        }

                        if (_targetEnemy != null && _targetEnemy == collider.transform)
                        {
                            closestEnemy = _targetEnemy;
                            closestDamageable = damageable;
                            break;
                        }

                        float distance = Vector2.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance && !_catsController.IsEnemyTargeted(damageable))
                        {
                            closestDistance = distance;
                            closestEnemy = collider.transform;
                            closestDamageable = damageable;
                        }
                    }
                }

                if (closestEnemy != null)
                {
                    if (_targetEnemy != null && _targetEnemy != closestEnemy &&
                        _targetEnemy.TryGetComponent(out IDamageable oldDamageable))
                    {
                        _catsController.ReleaseEnemyTarget(oldDamageable);
                    }

                    _targetEnemy = closestEnemy;
                    _catsController.AssignEnemyTarget(closestDamageable, this);
                    ChangeState(CatState.Running);
                }
            }
        }

        private void WalkAroundPlayer()
        {
            if (_playerTransform == null) return;

            float distanceToPlayer = Vector2.Distance(Transform.position, _playerTransform.position);

            if (distanceToPlayer > _maxDistanceFromPlayer)
            {
                Vector2 directionToPlayer =
                    ((Vector2)_playerTransform.position - (Vector2)Transform.position).normalized;
                Transform.position += (Vector3)directionToPlayer * (_speed * _returnToPlayerSpeedMultiplier * Time.deltaTime);
                FlipBasedOnDirection(directionToPlayer.x);
            }
            else
            {
                Vector2 randomDirection = new Vector2(
                    Mathf.Sin(Time.time * _walkFrequencyX + GetInstanceID()),
                    Mathf.Cos(Time.time * _walkFrequencyY + GetInstanceID())
                ).normalized;

                float speedMultiplier = distanceToPlayer < _minDistanceToPlayer
                    ? _nearPlayerSpeedMultiplier
                    : _walkSpeedMultiplier;

                Transform.position += (Vector3)randomDirection * (_speed * speedMultiplier * Time.deltaTime);
                FlipBasedOnDirection(randomDirection.x);
            }
        }

        private void RunTowardsEnemy()
        {
            if (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy)
            {
                if (_targetEnemy != null && _targetEnemy.TryGetComponent(out IDamageable damageable) &&
                    _catsController != null)
                {
                    _catsController.ReleaseEnemyTarget(damageable);
                }

                _targetEnemy = null;
                ChangeState(CatState.Walking);
                return;
            }

            float distanceToEnemy = Vector2.Distance(Transform.position, _targetEnemy.position);

            if (distanceToEnemy > _optimalAttackDistance + _attackDistanceThreshold)
            {
                Vector2 directionToEnemy = ((Vector2)_targetEnemy.position - (Vector2)Transform.position).normalized;
                Transform.position += (Vector3)directionToEnemy * (_speed * Time.deltaTime);
                FlipBasedOnDirection(directionToEnemy.x);
            }
            else
            {
                ChangeState(CatState.Attacking);
                StartCoroutine(AttackCooldown());
            }
        }

        private void MaintainAttackPosition()
        {
            if (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy) return;

            float distanceToEnemy = Vector2.Distance(Transform.position, _targetEnemy.position);

            if (distanceToEnemy < _optimalAttackDistance - _attackDistanceThreshold)
            {
                Vector2 directionFromEnemy = ((Vector2)Transform.position - (Vector2)_targetEnemy.position).normalized;
                Transform.position += (Vector3)directionFromEnemy * (_speed * _positionMaintenanceSpeedMultiplier * Time.deltaTime);
                FlipBasedOnDirection(-directionFromEnemy.x);
            }
            else if (distanceToEnemy > _optimalAttackDistance + _attackDistanceThreshold)
            {
                Vector2 directionToEnemy = ((Vector2)_targetEnemy.position - (Vector2)Transform.position).normalized;
                Transform.position += (Vector3)directionToEnemy * (_speed * _positionMaintenanceSpeedMultiplier * Time.deltaTime);
                FlipBasedOnDirection(directionToEnemy.x);
            }
            else
            {
                Vector2 directionToEnemy = ((Vector2)_targetEnemy.position - (Vector2)Transform.position).normalized;
                FlipBasedOnDirection(directionToEnemy.x);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out CharacterLogic.Character character))
            {
                HitEnemies.Add(damageable);
                damageable.TakeDamage(Damage);
            }
        }

        private IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(_attackCooldown);

            if (_targetEnemy != null && _targetEnemy.gameObject.activeInHierarchy)
            {
                float distanceToEnemy = Vector2.Distance(Transform.position, _targetEnemy.position);

                if (Mathf.Abs(distanceToEnemy - _optimalAttackDistance) <= _attackDistanceThreshold)
                {
                    if (_targetEnemy.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(Damage);
                    }

                    StartCoroutine(AttackCooldown());
                }
                else
                {
                    ChangeState(CatState.Running);
                }
            }
            else
            {
                if (_targetEnemy != null && _targetEnemy.TryGetComponent(out IDamageable damageable) &&
                    _catsController != null)
                {
                    _catsController.ReleaseEnemyTarget(damageable);
                }

                _targetEnemy = null;
                ChangeState(CatState.Walking);
            }
        }

        private void ChangeState(CatState newState)
        {
            _currentState = newState;
            _timeSinceLastStateChange = 0f;
            UpdateAnimatorState();
        }

        private void UpdateAnimatorState()
        {
            if (_animator == null) return;

            _animator.SetBool(WalkParam, false);
            _animator.SetBool(RunParam, false);
            _animator.SetBool(MeowParam, false);
            _animator.SetBool(IdleParam, false);

            switch (_currentState)
            {
                case CatState.Walking:
                    _animator.SetBool(WalkParam, true);
                    break;
                case CatState.Running:
                    _animator.SetBool(RunParam, true);
                    break;
                case CatState.Attacking:
                    _animator.SetBool(MeowParam, true);
                    break;
                case CatState.Idle:
                    _animator.SetBool(IdleParam, true);
                    break;
            }
        }

        private void FlipBasedOnDirection(float xDirection)
        {
            if (xDirection != 0)
            {
                SpriteRenderer.flipX = xDirection < 0;
            }
        }

        private IEnumerator ReturnToPoolAfterTime(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            PlayDisappearEffect();
        }

        private void PlayDisappearEffect()
        {
            if (_disappearEffect != null)
            {
                _disappearEffect.gameObject.SetActive(true);
                _disappearEffect.transform.position = transform.position;
                _disappearEffect.Play();

                ReleaseEnemyTarget();
                ReturnToPool();
            }
        }

        private void ReleaseEnemyTarget()
        {
            if (_targetEnemy != null && _targetEnemy.TryGetComponent(out IDamageable damageable) &&
                _catsController != null)
            {
                _catsController.ReleaseEnemyTarget(damageable);
                _targetEnemy = null;
            }
        }

        private void ReturnToPool()
        {
            ReleaseEnemyTarget();

            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }

            if (_stateCoroutine != null)
            {
                StopCoroutine(_stateCoroutine);
                _stateCoroutine = null;
            }
        }

        public override void ClearHitEnemies()
        {
            base.ClearHitEnemies();
            ReleaseEnemyTarget();
        }

        public Transform GetTargetEnemy()
        {
            return _targetEnemy;
        }

        private void OnDestroy()
        {
            if (_disappearEffect != null)
            {
                Destroy(_disappearEffect.gameObject);
            }
        }
    }
}