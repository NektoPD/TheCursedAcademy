using UnityEngine;

public class EnemyDamageTaker : MonoBehaviour, IDamageable
{
    private Health _health;
    private EnemyPool _pool;
    private Enemy _enemy;

    private void OnEnable()
    {
        if ( _health != null )
            _health.Died += Die;
    }

    private void OnDisable()
    {
        _health.Died -= Die;
    }

    public void Initialize(float maxHealth, EnemyPool pool, Enemy enemy)
    {
        _health = new Health(maxHealth);
        _pool = pool;
        _enemy = enemy;
    }

    public void TakeDamage(float damage) => _health.TakeDamage(damage);

    private void Die() => _pool.ReturnEnemy(_enemy);
}
