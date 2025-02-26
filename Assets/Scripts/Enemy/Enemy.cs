using UnityEngine;

[RequireComponent (typeof(EnemyMover), typeof(EnemyAttacker))]
[RequireComponent(typeof(EnemyDamageTaker), typeof(EnemyView))]
public class Enemy : MonoBehaviour, IPoolEntity
{
    private EnemyMover _mover;
    private EnemyAttacker _attacker;
    private EnemyView _view;
    private EnemyDamageTaker _damageTaker;

    private void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _attacker = GetComponent<EnemyAttacker>();
        _view = GetComponent<EnemyView>();
        _damageTaker = GetComponent<EnemyDamageTaker>();
    }

    private void OnEnable()
    {
        _mover.TargetInRange += Attack;
    }

    private void OnDisable()
    {
        _mover.TargetInRange -= Attack;
    }

    public void Initialize(IData<Enemy> data, EnemyPool pool)
    {
        EnemyData enemyData = data as EnemyData;

        _damageTaker.Initialize(enemyData.Health, pool, this);
        _mover.Initialize(enemyData.Speed, enemyData.AttackRange);
        _attacker.Initialize(enemyData.Cooldown, enemyData.Projectile);
        _view.Initialize(enemyData.Sprite, enemyData.AnimatorController);
    }

    public void Despawn() => gameObject.SetActive(false);

    public void ResetEntity() => gameObject.SetActive(true);

    private void Attack(Transform target) => _attacker.Attack(target);
}
