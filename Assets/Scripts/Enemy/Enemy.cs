using UnityEngine;

[RequireComponent (typeof(EnemyMover), typeof(EnemyView))]
[RequireComponent(typeof(EnemyDamageTaker))]
public abstract class Enemy : MonoBehaviour, IPoolEntity
{
    protected EnemyMover Mover;
    protected EnemyView View;
    protected EnemyDamageTaker DamageTaker;

    private EnemyPool _pool;

    public virtual void Initialize(IData<Enemy> data, EnemyPool pool)
    {
        EnemyData enemyData = data as EnemyData;

        DamageTaker.Initialize(enemyData.Health, enemyData.ExpPointData);
        Mover.Initialize(enemyData.Speed, enemyData.AttackRange);
        View.Initialize(enemyData.Sprite, enemyData.AnimatorController);

        _pool = pool;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        _pool.ReturnEntity(this);
    }

    public void ResetEntity() => gameObject.SetActive(true);
}
