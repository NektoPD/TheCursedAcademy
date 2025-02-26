using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData", order = 1)]
public class EnemyData : ScriptableObject, IData<Enemy>
{
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _cooldown;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private Enemy _prefab;
    [SerializeField] private ProjectileData _projectile;

    public float Health => _health;

    public float Speed => _speed;

    public float Damage => _damage;

    public float AttackRange => _attackRange;

    public float Cooldown => _cooldown;

    public Sprite Sprite => _sprite;

    public RuntimeAnimatorController AnimatorController => _animatorController;

    public Enemy Prefab => _prefab;

    public ProjectileData Projectile => _projectile;
}
