using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
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

        private int _level = 1;
        private float _damageMultiplier = 1f;
        private float _widthMultiplier = 1f;
        private ItemProjectilePool _projectilePool;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_penProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            PenProjectile penProjectile = _projectilePool.GetFromPool<PenProjectile>(transform.position, Quaternion.identity);

            float facingDirection = MovementHandler.IsMovingLeft() ? -1f : 1f;

            penProjectile.transform.position = new Vector2(
                transform.position.x + (_spawnOffset * facingDirection),
                transform.position.y);

            penProjectile.transform.localScale = new Vector3(
                _attackWidth * _widthMultiplier * Mathf.Abs(facingDirection),
                1.5f,
                1.5f);

            SpriteRenderer spriteRenderer = penProjectile.SpriteRenderer;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0;
            }

            penProjectile.Initialize(Data.Damage * _damageMultiplier, this);
            penProjectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(penProjectile, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;
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