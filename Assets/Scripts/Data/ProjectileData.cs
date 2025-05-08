using EnemyLogic.ProjectileLogic;
using UnityEngine;

namespace Data.ProjectilesData
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectiles/ProjectileData", order = 1)]
    public class ProjectileData : ScriptableObject, IData<Projectile>
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public Projectile Prefab { get; private set; }
    }
}
