using System;
using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Enums;
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

        [Header("Level Up Settings")]
        [SerializeField] private float _durationIncreasePerLevel = 0.25f;
        [SerializeField] private float _cooldownReductionPerLevel = 0.9f;

        private int _level = 1;
        private float _durationMultiplier = 1f;
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

            _durationMultiplier = 1f;
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

        public override void LevelUp()
        {
            _level++;

            _durationMultiplier += _durationIncreasePerLevel;
            Data.Cooldown *= _cooldownReductionPerLevel;
            
            UpdateStatsValues();
        }
        
        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(StatVariations.AttackSpeed, Data.Cooldown);
            ItemStats.SetStatCurrentValue(StatVariations.Duration, _durationMultiplier);

            ItemStats.SetStatNextValue(StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
            ItemStats.SetStatNextValue(StatVariations.Duration, _durationMultiplier + _durationIncreasePerLevel);
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