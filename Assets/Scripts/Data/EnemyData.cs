using UnityEngine;

public abstract class EnemyData : ScriptableObject, IData<Enemy>
{
    [SerializeField] private int _id;
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _cooldown;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private ExpPointData _expPointData;

    public int Id => _id;

    public float Health => _health;

    public float Speed => _speed;

    public float Damage => _damage;

    public float AttackRange => _attackRange;

    public float Cooldown => _cooldown;

    public Sprite Sprite => _sprite;

    public RuntimeAnimatorController AnimatorController => _animatorController;

    public ExpPointData ExpPointData => _expPointData;

    public abstract Enemy Prefab { get; }
}
