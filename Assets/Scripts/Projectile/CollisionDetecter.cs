using UnityEngine;

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

        if (collision.TryGetComponent(out Projectile _))
            return;

        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
            _view.SetHitTrigger();
        }
    }

    public void Initialize(float damage) => _damage = damage;
}
