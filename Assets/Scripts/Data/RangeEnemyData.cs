using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/RangeEnemyData", order = 2)]
public class RangeEnemyData : EnemyData
{
    [SerializeField] private ProjectileData _projectileData;
    [SerializeField] private RangeEnemy _prefab;

    public ProjectileData ProjectileData => _projectileData;

    public override Enemy Prefab => _prefab;
}
