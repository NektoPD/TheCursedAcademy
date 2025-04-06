using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ItemProjectilePool))]
    public class Parfume : Item
    {
        [SerializeField] private ParfumeProjectile _parfumeProjectilePrefab;
        [SerializeField] private float _projectileLifetime = 10f;
        [SerializeField] private float _damageZoneDuration = 10f;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private float _minSpawnDistance = 8f;
        [SerializeField] private float _maxSpawnDistance = 12f;
        [SerializeField] private float _minScreenOffset = 0.1f;
        [SerializeField] private float _maxScreenOffset = 0.3f;
        [SerializeField] private float _projectileSpeed = 5f;
        [SerializeField] private float _minScreenInsetX = 0.2f;
        [SerializeField] private float _maxScreenInsetX = 0.8f;
        [SerializeField] private float _minScreenInsetY = 0.2f;
        [SerializeField] private float _maxScreenInsetY = 0.8f;

        private int _level = 1;

        /*private float _damageMultiplier = 1f;
        private float _radiusMultiplier = 1f;*/
        private ItemProjectilePool _projectilePool;
        private Camera _mainCamera;

        private void Awake()
        {
            _projectilePool = GetComponent<ItemProjectilePool>();
            _projectilePool.Initialize(_parfumeProjectilePrefab, _initialPoolSize);
            _mainCamera = Camera.main;
        }

        protected override void PerformAttack()
        {
            Vector2 spawnPosition = GetRandomPositionOutsideScreen();

            Vector2 targetPosition = GetRandomPositionInsideScreen();

            ParfumeProjectile parfumeProjectile =
                _projectilePool.GetFromPool<ParfumeProjectile>(spawnPosition, Quaternion.identity);

            parfumeProjectile.Initialize(Data.Damage, this);
            parfumeProjectile.SetupMovement(targetPosition, _projectileSpeed, _damageZoneDuration);
            parfumeProjectile.ClearHitEnemies();

            StartCoroutine(DisableProjectileAfterLifetime(parfumeProjectile, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;
        }

        private Vector2 GetRandomPositionOutsideScreen()
        {
            Vector2 viewportPosition;
            Vector2 worldPosition;

            int edge = Random.Range(0, 4);

            switch (edge)
            {
                case 0:
                    viewportPosition = new Vector2(Random.value, 1f + Random.Range(_minScreenOffset, _maxScreenOffset));
                    break;
                case 1:
                    viewportPosition = new Vector2(1f + Random.Range(_minScreenOffset, _maxScreenOffset), Random.value);
                    break;
                case 2:
                    viewportPosition = new Vector2(Random.value, -Random.Range(_minScreenOffset, _maxScreenOffset));
                    break;
                case 3:
                    viewportPosition = new Vector2(-Random.Range(_minScreenOffset, _maxScreenOffset), Random.value);
                    break;
                default:
                    viewportPosition = new Vector2(1f + Random.Range(_minScreenOffset, _maxScreenOffset), Random.value);
                    break;
            }

            worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);

            return worldPosition;
        }

        private Vector2 GetRandomPositionInsideScreen()
        {
            Vector2 viewportPosition = new Vector2(
                Random.Range(_minScreenInsetX, _maxScreenInsetX),
                Random.Range(_minScreenInsetY, _maxScreenInsetY)
            );

            Vector2 worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);

            return worldPosition;
        }

        private IEnumerator DisableProjectileAfterLifetime(ParfumeProjectile projectile, float lifetime)
        {
            yield return new WaitForSeconds(lifetime);

            if (projectile && projectile.gameObject.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }
    }
}