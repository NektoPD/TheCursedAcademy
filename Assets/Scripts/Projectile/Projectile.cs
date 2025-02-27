using UnityEngine;

[RequireComponent (typeof(ProjectileView), typeof(CollisionDetecter))]
public class Projectile : MonoBehaviour, IPoolEntity
{
    private ProjectileData _projectileData;
    private Vector3 _direction;
    private ProjectileView _view;
    private ProjectilePool _pool;
    private CollisionDetecter _detecter;

    private void OnEnable()
    {
        _view = GetComponent<ProjectileView>();
        _detecter = GetComponent<CollisionDetecter>();
    }

    public void Initialize(IData<Projectile> data, ProjectilePool pool)
    {
        ProjectileData projectileData = data as ProjectileData;

        _projectileData = projectileData;
        _pool = pool;
        _view.Initialize(projectileData.Sprite, projectileData.AnimatorController);
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void SetDamage(float damage) => _detecter.Initialize(damage);
     
    private void FixedUpdate()
    {
        transform.position += _projectileData.Speed * Time.fixedDeltaTime * _direction;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        _pool.ReturnEntity(this);
    }

    public void ResetEntity() => gameObject.SetActive(true);
}
