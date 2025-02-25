using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectiles/ProjectileData", order = 1)]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private float _speed;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Projectile _prefab;

    public float Speed => _speed;

    public Sprite Sprite => _sprite;    
}
