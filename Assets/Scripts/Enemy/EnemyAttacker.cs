using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyMover), typeof(EnemyAnimator))]
public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    private EnemyMover _mover;
    private EnemyAnimator _view;
    private AttackerManager _attackerManager;

    private Coroutine _coroutine;
    private Transform _target;
    private Transform _currentProjectileSpawnPoint;

    private IReadOnlyList<AttackData> _attacksData;
    private AttackData _currentAttack;

    private bool _canAttack = true;

    public Vector3 CurrentProjectileSpawnPosition => _currentProjectileSpawnPoint.position;

    public IReadOnlyList<Transform> EnemySpawnPoints => _enemySpawnPoints;

    [Inject]
    private void Construct(AttackerManager attackerManager)
    {
        _attackerManager = attackerManager;
    }

    private void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _view = GetComponent<EnemyAnimator>();
    }

    private void OnEnable()
    {
        _mover.TargetInRange += TryAttack;
    }

    private void OnDisable()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        _mover.TargetInRange -= TryAttack;
    }

    public void Initialize(IReadOnlyList<AttackData> attacksData)
    {
        _canAttack = true;
        _attacksData = attacksData;
        _currentAttack = _attacksData[Random.Range(0, _attacksData.Count)];
        _mover.SetAttackRange(_currentAttack.AttackRange);
        _currentProjectileSpawnPoint = _projectileSpawnPoint;
    }

    private void TryAttack(Transform target)
    {
        _target = target;

        if (_canAttack)
        {
            _canAttack = false;

            _view.SetTriggerByName(_currentAttack.NameInAnimator);

            _coroutine = StartCoroutine(Reload(_currentAttack.Cooldown));
        }
    }

    private void AttackToggle() => _attackerManager.ExecuteAttack(_currentAttack, this);

    private void SetProjectileSpawnPointOnTarget() => _currentProjectileSpawnPoint = _target;

    private IEnumerator Reload(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        _currentAttack = _attacksData[Random.Range(0, _attacksData.Count)];
        _mover.SetAttackRange(_currentAttack.AttackRange);
        _canAttack = true;
    }
}
