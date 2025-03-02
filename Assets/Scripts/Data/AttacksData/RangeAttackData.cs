using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttackData", menuName = "Attacks/RangeAttackData ", order = 2)]
public class RangeAttackData : AttackData
{
    [SerializeField] private float _damage;
    [SerializeField] private ProjectileData _projectile; 

    public float Damage => _damage;

    public ProjectileData ProjectileData => _projectile;
}
