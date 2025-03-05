using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyMover), typeof(EnemyAnimator))]
public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private List<Transform> _projectileSpawnPoints;
    [SerializeField] private List<Transform> _enemySpawnPoints;

    private EnemyMover _mover;
    private EnemyAnimator _view;
    private AttackerManager _attackerManager;

    private Transform _target;
    private Transform _currentProjectileSpawnPoint;

    private List<AttackData> _attacksData;
    private AttackData _currentAttack;

    public IReadOnlyList<Transform> ProjectileSpawnPoints => _projectileSpawnPoints;

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
        _mover.TargetInRange -= TryAttack;
    }

    public void Initialize(IReadOnlyList<AttackData> attacksData)
    {
        _attacksData = attacksData.ToList();

        SetAttack();

        _currentProjectileSpawnPoint = _projectileSpawnPoints.First();
    }

    private void TryAttack(Transform target)
    {
        _target = target;

        if (_currentAttack != null)
        {
            _view.SetTriggerByName(_currentAttack.NameInAnimator);
            StartCoroutine(Reload(_currentAttack.Cooldown, _currentAttack));
        }
        else
        {
            SetAttack();
        }
    }

    private void AttackToggle()
    {
        _attackerManager.ExecuteAttack(_currentAttack, this);
        _currentAttack = null;
    }
     
    private void SetProjectileSpawnPointOnTarget() => _currentProjectileSpawnPoint = _target;

    private IEnumerator Reload(float cooldown, AttackData attack)
    {
        yield return new WaitForSeconds(cooldown);

        _attacksData.Add(attack);
    }

    private bool TryGetNewAttack(out AttackData attack)
    {
        attack = null;

        if (_attacksData.Count != 0)
        {
            attack = _attacksData[Random.Range(0, _attacksData.Count)];
            _attacksData.Remove(attack);
        }

        return attack != null;
    }

    private void SetAttack()
    {
        if (TryGetNewAttack(out AttackData newAttack))
        {
            _currentAttack = newAttack;
            _mover.SetAttackRange(_currentAttack.AttackRange);
        }
    }
}
