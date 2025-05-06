using System;
using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class BackpackItem : Item
    {
        [SerializeField] private BackpackItemProjectile _backpackProjectilePrefab;
        [SerializeField] private float _effectDuration = 3f;
        [SerializeField] private int _initialPoolSize = 1;
        [SerializeField] private float _durationMultiplier = 1f;

        private int _level = 1;
        private ItemProjectilePool _projectilePool;
        private BackpackItemProjectile _activeProjectile;
        private Coroutine _activeEffectCoroutine;
        private Transform _transform;

        public event Action InvincibilityEnabled;
        public event Action InvincibilityDisabled;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_backpackProjectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        protected override void PerformAttack()
        {
            if (_activeEffectCoroutine != null)
            {
                StopCoroutine(_activeEffectCoroutine);
                if (_activeProjectile != null && _activeProjectile.gameObject.activeSelf)
                {
                    _projectilePool.ReturnToPool(_activeProjectile);
                    _activeProjectile = null;
                }
            }

            Vector3 spawnPosition = _transform.position;
            _activeProjectile = _projectilePool.GetFromPool<BackpackItemProjectile>(spawnPosition, Quaternion.identity);

            _activeProjectile.Initialize(Data.Damage, this);
            InvincibilityEnabled?.Invoke();

            float effectDuration = _effectDuration * _durationMultiplier;
            _activeProjectile.PlayParticleEffect(effectDuration);

            _activeEffectCoroutine = StartCoroutine(DisableEffectAfterDuration(effectDuration));
        }

        protected override void LevelUp()
        {
            _level++;
            _durationMultiplier += 0.25f;
        }

        private IEnumerator DisableEffectAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (_activeProjectile != null && _activeProjectile.gameObject.activeSelf)
            {
                InvincibilityDisabled?.Invoke();
                _projectilePool.ReturnToPool(_activeProjectile);
                _activeProjectile = null;
            }

            _activeEffectCoroutine = null;
        }

        private void OnDisable()
        {
            if (_activeEffectCoroutine != null)
            {
                StopCoroutine(_activeEffectCoroutine);

                if (_activeProjectile != null && _activeProjectile.gameObject.activeSelf)
                {
                    InvincibilityDisabled?.Invoke();
                    _projectilePool.ReturnToPool(_activeProjectile);
                    _activeProjectile = null;
                }

                _activeEffectCoroutine = null;
            }
        }
    }
}