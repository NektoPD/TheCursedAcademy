using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(EnemyView))]
public abstract class EnemyAttacker : MonoBehaviour
{
    protected Coroutine Coroutine;
    protected float Cooldown;
    protected float Damage;
    protected Transform Target;
    private EnemyMover _mover;
    private EnemyView _view;

    private bool _canAttack = true;

    private void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _view = GetComponent<EnemyView>();
    }

    private void OnEnable()
    {
        Coroutine = StartCoroutine(Reload());
        _mover.TargetInRange += Attack;
    }

    private void OnDisable()
    {
        StopCoroutine(Coroutine);
        _mover.TargetInRange -= Attack;
    }

    private void Attack(Transform target)
    {
        Target = target;

        if (_canAttack)
        {
            _canAttack = false;

            _view.SetAttackTrigger();

            Coroutine = StartCoroutine(Reload());
        }
    }

    protected abstract void AttackToggle();

    protected IEnumerator Reload()
    {
        yield return new WaitForSeconds(Cooldown);

        _canAttack = true;
    }
}
