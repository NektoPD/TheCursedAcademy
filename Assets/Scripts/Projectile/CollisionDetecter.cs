using UnityEngine;

[RequireComponent(typeof(ProjectileView))]
public class CollisionDetecter : MonoBehaviour
{
    private ProjectileView _view;
    private float _damage;

    private void Awake()
    {
        _view = GetComponent<ProjectileView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable) && collision.TryGetComponent(out Enemy _) == false)
        {
            _view.SetHitTrigger();
            damageable.TakeDamage(_damage);
        }
    }

    public void Initialize(float damage) => _damage = damage;
}
