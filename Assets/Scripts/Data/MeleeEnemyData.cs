using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/MeleeEnemyData", order = 1)]
public class MeleeEnemyData : EnemyData
{
    [SerializeField] private MeleeEnemy _prefab;

    public override Enemy Prefab => _prefab;
}
