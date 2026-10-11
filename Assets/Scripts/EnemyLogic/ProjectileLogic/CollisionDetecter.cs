using CharacterLogic;
using HealthSystem;
using UnityEngine;

namespace EnemyLogic.ProjectileLogic
{
    [RequireComponent(typeof(ProjectileView))]
    public class CollisionDetecter : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayer;

        private ProjectileView _view;
        private float _damage;
        private bool _hasHit;

        private void Awake()
        {
            _view = GetComponent<ProjectileView>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasHit)
                return;

            if ((_enemyLayer.value & (1 << collision.gameObject.layer)) != 0)
                return;

            if (collision.TryGetComponent<CharacterCollisionHandler>(out _))
                return;

            if (collision.TryGetComponent(out Projectile _))
                return;

            Character character = collision.GetComponentInParent<Character>();
            IDamageable damageable;

            if (character != null)
            {
                if (collision != character.DamageCollider)
                    return;

                damageable = character;
            }
            else
            {
                damageable = collision.GetComponent<IDamageable>();
            }

            if (damageable != null)
            {
                _hasHit = true;
                damageable.TakeDamage(_damage);
                _view.SetHitTrigger();
            }
        }

        public void Initialize(float damage)
        {
            _damage = damage;
            _hasHit = false;
        }
    }
}
