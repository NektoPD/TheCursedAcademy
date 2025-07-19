using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Enums;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class GiantPen : Item
    {
        [SerializeField] private PenProjectile _penProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 2f;
        [SerializeField] private float _attackWidth = 1.5f;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private float _spawnOffset = 0.5f;
        [SerializeField] private Vector3 _projectileScale = new Vector3(1f, 1.5f, 1.5f);
        [SerializeField] private float _damageMultiplier = 1f;
        [SerializeField] private float _widthMultiplier = 2f;

        private ItemProjectilePool _projectilePool;
        private bool _lastMovementWasLeft = false;
        private Transform _transform;
        private float _damageIncreasePerLevel = 1.3f;
        private float _cooldownReductionPerLevel = 0.85f;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_penProjectilePrefab, _initialPoolSize);
            _transform = transform;
        }

        private void Update()
        {
            if (MovementHandler.IsMoving())
            {
                _lastMovementWasLeft = MovementHandler.IsMovingLeft();
            }
        }

        protected override void PerformAttack()
        {
            bool isMovingLeft = MovementHandler.IsMoving()
                ? MovementHandler.IsMovingLeft()
                : _lastMovementWasLeft;

            float facingDirection = isMovingLeft ? -1f : 1f;

            Vector3 spawnPosition = new Vector3(
                _transform.position.x + (_spawnOffset * facingDirection),
                _transform.position.y,
                _transform.position.z);

            PenProjectile penProjectile =
                _projectilePool.GetFromPool<PenProjectile>(spawnPosition, Quaternion.identity);

            penProjectile.Transform.localScale = new Vector3(
                _attackWidth * _widthMultiplier * Mathf.Abs(facingDirection),
                _projectileScale.y,
                _projectileScale.z);

            SpriteRenderer spriteRenderer = penProjectile.SpriteRenderer;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = isMovingLeft;
            }

            penProjectile.Initialize(Data.Damage * _damageMultiplier, this);
            penProjectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(penProjectile, _projectileLifetime));
        }

        public override void LevelUp()
        {
            Level++;

            Data.Cooldown *= _cooldownReductionPerLevel;
            _damageMultiplier += _damageIncreasePerLevel;

            UpdateStatsValues();
        }

        protected override void UpdateStatsValues()
        {
            ItemStats.SetStatCurrentValue(StatVariations.Damage, _damageMultiplier);
            ItemStats.SetStatCurrentValue(StatVariations.AttackSpeed, Data.Cooldown);

            ItemStats.SetStatNextValue(StatVariations.Damage, _damageMultiplier + _damageIncreasePerLevel);
            ItemStats.SetStatNextValue(StatVariations.AttackSpeed, Data.Cooldown * _cooldownReductionPerLevel);
        }

        private IEnumerator EnableProjectile(ItemProjectile projectile, float lifetime)
        {
            float timer = 0f;

            projectile.gameObject.SetActive(true);

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }
    }
}