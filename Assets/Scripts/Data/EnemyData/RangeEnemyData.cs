using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/RangeEnemyData", order = 2)]
public class RangeEnemyData : MeleeEnemyData
{
    [SerializeField] private ProjectileData _projectileData;

    public ProjectileData ProjectileData => _projectileData;
}
