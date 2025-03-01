using UnityEngine;

public class MeleeAttacker : EnemyAttacker
{
    private float _radius;

    public void Initialize(float cooldown, float radius, float damage)
    {
        Cooldown = cooldown;
        _radius = radius;
        Damage = damage;
    }

    protected override void AttackToggle()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius);

        foreach (var hit in hits)
            if (hit.TryGetComponent(out IDamageable damageable) && hit.TryGetComponent(out Enemy _) == false)
                damageable.TakeDamage(Damage);
    }
}

