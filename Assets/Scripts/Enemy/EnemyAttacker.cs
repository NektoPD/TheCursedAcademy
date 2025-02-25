using System.Collections;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    private ProjectileData _projectile;
    private Coroutine _coroutine;
    private float _cooldown;

    private bool _canAttack = true;

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
        _projectile = projectile;
    }

    public void Attack(Transform target)
    {
        if (_canAttack)
        {
            _canAttack = false;

            Debug.Log("fire");

            _coroutine = StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(_cooldown);

        _canAttack = true;
    }
}
