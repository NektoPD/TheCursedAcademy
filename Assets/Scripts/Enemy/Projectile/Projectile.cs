using UnityEngine;

[RequireComponent (typeof(ProjectileView))]
public class Projectile : MonoBehaviour
{
    private ProjectileData _projectileData;
    private Vector2 _direction;
    private ProjectileView _view;

    private void OnEnable()
    {
        _view = GetComponent<ProjectileView>();
    }

    public void Initialize(ProjectileData projectileData, Vector2 direction)
    {
        _projectileData = projectileData;
        _direction = direction;
        _view.Initialize(projectileData.Sprite);
    }
     
    private void FixedUpdate()
    {
        transform.Translate(_projectileData.Speed * Time.fixedDeltaTime * _direction);
    }

    public void Despawn() => gameObject.SetActive(false);

    public void ResetProjectile() => gameObject.SetActive(true);
}
