using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private Transform _projectileSpawnPoint;

    private ProjectileData _projectileData;
    private Coroutine _coroutine;
    private float _cooldown;
    private ProjectileSpawner _projectileSpawner;

    private bool _canAttack = true;

    [Inject]
    public void Construct(ProjectileSpawner projectileSpawner)
    {
        _projectileSpawner = projectileSpawner;
    }

    private void OnEnable()
    {
        _coroutine = StartCoroutine(Reload());
    }

    private void OnDisable()
    {
        StopCoroutine(_coroutine);
    }

    public void Initialize(float cooldowm, ProjectileData projectile)
    {
        _cooldown = cooldowm;
        _projectileData = projectile;
    }

    public void Attack(Transform target)
    {
        if (_canAttack)
        {
            _canAttack = false;

            SpawnProjectile(target);

            _coroutine = StartCoroutine(Reload());
        }
    }

    private void SpawnProjectile(Transform target)
    {
        Projectile projectile = _projectileSpawner.Spawn(_projectileData);

        projectile.transform.position = _projectileSpawnPoint.position;

        projectile.SetDirection((target.position - projectile.transform.position).normalized);
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(_cooldown);

        _canAttack = true;
    }
}
