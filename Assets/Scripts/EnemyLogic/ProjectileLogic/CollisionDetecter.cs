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

        private void Awake()
        {
            _view = GetComponent<ProjectileView>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((_enemyLayer.value & (1 << collision.gameObject.layer)) != 0)
                return;

            if (collision.TryGetComponent<CharacterCollisionHandler>(out _))
                return;

            if (collision.TryGetComponent(out Projectile _))
                return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable ??= collision.GetComponentInParent<Character>();

            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
                _view.SetHitTrigger();
            }
        }

        public void Initialize(float damage) => _damage = damage;
    }
}
