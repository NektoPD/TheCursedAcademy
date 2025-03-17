using UnityEngine;
using Zenject;

[RequireComponent (typeof(HealthBar), typeof(EnemyAnimator))]
public class EnemyDamageTaker : MonoBehaviour, IDamageable
{
    private Health _health;
    private HealthBar _healthBar;
    private EnemyAnimator _enemyAnimator;
    private ExpPointPool _expPointPool;
    private ExpPointData _expPointData;

    private bool _isDied = false;

    [Inject]
    public void Construct(ExpPointPool expPointPool)
    {
        _expPointPool = expPointPool;
    }

    private void Awake()
    {
        _healthBar = GetComponent<HealthBar>();
        _enemyAnimator = GetComponent<EnemyAnimator>();
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.Died -= Die;
    }

    public void Initialize(float maxHealth, ExpPointData expPointData)
    {
        _isDied = false;
        _health = new Health(maxHealth);
        _healthBar.SetHealth(_health);
        _expPointData = expPointData;

        _health.Died += Die;
    }

    public void TakeDamage(float damage)
    {
        if (_isDied)
            return;

        _enemyAnimator.SetHurtTigger();
        _health.TakeDamage(damage);
    }

    private void Die()
    {
        _isDied = true;
        ExpPoint point = _expPointPool.Get(_expPointData);
        point.transform.position = transform.position;
        _enemyAnimator.SetDeadTrigger();
    }
}
