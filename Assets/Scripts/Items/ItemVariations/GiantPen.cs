using System.Collections;
using CharacterLogic.InputHandler;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(ProjectilePool))]
    public class GiantPen : Item
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private float _projectileLifetime = 2f;
        [SerializeField] private float _attackWidth = 1.5f;
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

            float facingDirection = MovementHandler && MovementHandler.IsMovingLeft() ? -1f : 1f;
            
            projectile.transform.position = new Vector2(
                transform.position.x + (_attackWidth * _widthMultiplier * facingDirection), 
                transform.position.y);
                
            projectile.transform.localScale = new Vector3(1.5f * facingDirection, 1.5f, 1.5f);

            projectile.Initialize(Data.Damage * _damageMultiplier, this);

            projectile.ClearHitEnemies();

            StartCoroutine(EnableProjectile(projectile, _projectileLifetime));
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

        private IEnumerator EnableProjectile(Projectile projectile, float lifetime)
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