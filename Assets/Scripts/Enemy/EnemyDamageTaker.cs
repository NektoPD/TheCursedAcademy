using UnityEngine;
using Zenject;

[RequireComponent (typeof(EnemyView))]
public class EnemyDamageTaker : MonoBehaviour, IDamageable
{
    private Health _health;
    private EnemyView _enemyView;
    private ExpPointPool _expPointPool;
    private ExpPointData _expPointData;

    [Inject]
    public void Construct(ExpPointPool expPointPool)
    {
        _expPointPool = expPointPool;
    }

    private void Awake()
    {
        _enemyView = GetComponent<EnemyView>();
    }

    private void OnDisable()
    {
        _health.Died -= Die;
    }

    public void Initialize(float maxHealth, ExpPointData expPointData)
    {
        _health = new Health(maxHealth);
        _enemyView.SetHealth(_health);
        _expPointData = expPointData;

        _health.Died += Die;
    }

    public void TakeDamage(float damage)
    {
        _enemyView.SetHurtTigger();
        _health.TakeDamage(damage);
    }

    private void Die()
    {
        _enemyView.SetDeadTrigger();
        ExpPoint point = _expPointPool.Get(_expPointData);
        point.transform.position = transform.position;
    }
}
