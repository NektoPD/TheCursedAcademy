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
        [SerializeField] private ItemProjectile _itemProjectilePrefab;
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
            _projectilePool.Initialize(_itemProjectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            ItemProjectile itemProjectile = _projectilePool.GetFromPool(transform.position, Quaternion.identity);

            float facingDirection = MovementHandler.IsMovingLeft() ? -1f : 1f;

            itemProjectile.transform.position = new Vector2(
                transform.position.x + (_spawnOffset * facingDirection),
                transform.position.y);

            itemProjectile.transform.localScale = new Vector3(
                _attackWidth * _widthMultiplier * Mathf.Abs(facingDirection),
                1.5f,
                1.5f);

            SpriteRenderer spriteRenderer = itemProjectile.SpriteRenderer;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0;
            }

            itemProjectile.Initialize(Data.Damage * _damageMultiplier, this);

            itemProjectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(itemProjectile, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;
            
        }

        private IEnumerator EnableProjectile(ItemProjectile itemProjectile, float lifetime)
        {
            float timer = 0f;

            itemProjectile.gameObject.SetActive(true);

            while (timer < lifetime && itemProjectile && itemProjectile.gameObject.activeSelf)
            {
                timer += Time.deltaTime;

                yield return null;
            }

            if (itemProjectile && itemProjectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(itemProjectile);
            }
        }
    }
}