using System.Collections;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.Toys
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Toys : Item
    {
        [SerializeField] private ToysProjectile _toysProjectilePrefab;
        [SerializeField] private int _baseProjectileCount = 2;
        [SerializeField] private int _maxProjectileCount = 5;
        [SerializeField] private float _rotationRadius = 1f;
        [SerializeField] private float _initialAngleStep = 120f;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private int _initialPoolSize = 5;
        [SerializeField] private float _scaleDownDuration = 0.5f;
        [SerializeField] private float _scaleUpDuration = 0.5f;
        [SerializeField] private float _delayBeforeNextAttack = 0.5f;

        [Header("Level Up Settings")]
        [SerializeField] private float _baseDamageMultiplier = 1f;
        [SerializeField] private float _damageIncreasePerLevel = 0.15f;
        [SerializeField] private float _radiusIncreasePerLevel = 0.1f;
        [SerializeField] private float _rotationSpeedIncreasePerLevel = 15f;
        [SerializeField] private float _cooldownReductionPerLevel = 0.95f;

        private int _level = 1;
        private int _currentProjectileCount;
        private float _damageMultiplier = 1f;
        private float _radiusMultiplier = 1f;
        private float _rotationSpeedMultiplier = 1f;
        private ItemProjectilePool _projectilePool;
        private ToysProjectile[] _activeProjectiles;
        private Coroutine _attackCycleCoroutine;
        private Transform _transform;
        private bool _isAttackReady = true;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_toysProjectilePrefab, _initialPoolSize);
            _currentProjectileCount = _baseProjectileCount;
            _activeProjectiles = new ToysProjectile[_maxProjectileCount];
            _transform = transform;

            _damageMultiplier = _baseDamageMultiplier;
            _radiusMultiplier = 1f;
            _rotationSpeedMultiplier = 1f;
            
            UpdateAngleStep();
        }

        private void UpdateAngleStep()
        {
            _initialAngleStep = 360f / _currentProjectileCount;
        }

        protected override void PerformAttack()
        {
            if (!_isAttackReady) return;

            if (_attackCycleCoroutine != null)
            {
                StopCoroutine(_attackCycleCoroutine);
            }

            _isAttackReady = false;
            _attackCycleCoroutine = StartCoroutine(AttackCycle());
        }

        private IEnumerator AttackCycle()
        {
            SpawnProjectiles();

            yield return new WaitForSeconds(_delayBeforeNextAttack);

            DeactivateAllProjectiles();

            yield return new WaitForSeconds(Data.Cooldown);

            _isAttackReady = true;
        }

        private void SpawnProjectiles()
        {
            for (int i = 0; i < _currentProjectileCount; i++)
            {
                ToysProjectile projectile =
                    _projectilePool.GetFromPool<ToysProjectile>(_transform.position, Quaternion.identity);
                _activeProjectiles[i] = projectile;

                float initialAngle = i * _initialAngleStep;

                projectile.Initialize(Data.Damage * _damageMultiplier, this);
                projectile.ClearHitEnemies();

                Vector3 originalScale = projectile.Transform.localScale;
                projectile.Transform.localScale = Vector3.zero;
                projectile.gameObject.SetActive(true);

                StartCoroutine(ScaleUpProjectile(projectile, originalScale));

                StartCoroutine(RotateProjectile(
                    projectile,
                    initialAngle,
                    _delayBeforeNextAttack,
                    _rotationRadius * _radiusMultiplier));
            }
        }

        private void DeactivateAllProjectiles()
        {
            for (int i = 0; i < _currentProjectileCount; i++)
            {
                if (_activeProjectiles[i] != null && _activeProjectiles[i].gameObject.activeSelf)
                {
                    StartCoroutine(ScaleDownAndReturn(_activeProjectiles[i]));
                }
            }
        }

        public override void LevelUp()
        {
            _level++;

            _damageMultiplier += _damageIncreasePerLevel;

            _radiusMultiplier += _radiusIncreasePerLevel;

            _rotationSpeedMultiplier += _rotationSpeedIncreasePerLevel / _rotationSpeed;
            _rotationSpeed = 180f * _rotationSpeedMultiplier;

            Data.Cooldown *= _cooldownReductionPerLevel;

            if (_level % 2 == 0 && _currentProjectileCount < _maxProjectileCount)
            {
                _currentProjectileCount++;
                UpdateAngleStep();
            }
        }

        private IEnumerator RotateProjectile(ToysProjectile projectile, float initialAngle, float duration,
            float radius)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration && projectile && projectile.gameObject.activeSelf)
            {
                float currentAngle = initialAngle + (_rotationSpeed * elapsedTime);
                float x = _transform.position.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                float y = _transform.position.y + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

                projectile.Transform.position = new Vector3(x, y, _transform.position.z);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator ScaleUpProjectile(ToysProjectile projectile, Vector3 targetScale)
        {
            Vector3 startScale = Vector3.zero;
            float elapsedTime = 0f;

            while (elapsedTime < _scaleUpDuration)
            {
                projectile.Transform.localScale =
                    Vector3.Lerp(startScale, targetScale, elapsedTime / _scaleUpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            projectile.Transform.localScale = targetScale;
        }

        private IEnumerator ScaleDownAndReturn(ToysProjectile projectile)
        {
            Vector3 originalScale = projectile.Transform.localScale;
            Vector3 targetScale = Vector3.zero;
            float elapsedTime = 0f;

            while (elapsedTime < _scaleDownDuration)
            {
                projectile.Transform.localScale =
                    Vector3.Lerp(originalScale, targetScale, elapsedTime / _scaleDownDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            projectile.Transform.localScale = targetScale;
            _projectilePool.ReturnToPool(projectile);
            projectile.Transform.localScale = originalScale;
        }

        private void OnDisable()
        {
            if (_attackCycleCoroutine != null)
            {
                StopCoroutine(_attackCycleCoroutine);
                _attackCycleCoroutine = null;
            }

            for (int i = 0; i < _maxProjectileCount; i++)
            {
                if (_activeProjectiles[i] != null && _activeProjectiles[i].gameObject.activeSelf)
                {
                    _projectilePool.ReturnToPool(_activeProjectiles[i]);
                }
            }
        }
    }
}