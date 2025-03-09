using System.Collections;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    public class GiantPen : Item
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLifetime = 2f;
        [SerializeField] private float _attackWidth = 3f;
        [SerializeField] private int _initialPoolSize = 3;

        private int _level = 1;
        private float _damageMultiplier = 1f;
        private float _widthMultiplier = 1f;
        private ProjectilePool _projectilePool;

        private void Awake()
        {
            _projectilePool = GetComponent<ProjectilePool>();
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            Projectile projectile = _projectilePool.GetFromPool(transform.position, Quaternion.identity);

            projectile.transform.localScale = new Vector3(_attackWidth * _widthMultiplier, 1f, 1f);

            projectile.Initialize(Data.Damage * _damageMultiplier, this);

            projectile.ClearHitEnemies();

            StartCoroutine(MoveProjectile(projectile, transform.right, _projectileSpeed, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;

            _damageMultiplier = 1f + (_level - 1) * 0.25f;

            if (_level % 3 == 0)
            {
                _widthMultiplier += 0.2f;
            }

            if (_level % 5 == 0 && _level <= 25)
            {
                Data.Cooldown *= 0.9f;
            }
        }

        private IEnumerator MoveProjectile(Projectile projectile, Vector3 direction, float speed, float lifetime)
        {
            float timer = 0f;

            while (timer < lifetime && projectile && projectile.gameObject.activeSelf)
            {
                projectile.transform.position += direction * (speed * Time.deltaTime);
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