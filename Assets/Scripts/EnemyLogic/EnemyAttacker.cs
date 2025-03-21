using Data;
using Zenject;
using System.Linq;
using UnityEngine;
using EnemyLogic.Attackers;
using System.Collections;
using System.Collections.Generic;

namespace EnemyLogic
{
    [RequireComponent(typeof(EnemyMover), typeof(EnemyAnimator))]
    public class EnemyAttacker : MonoBehaviour
    {
        [SerializeField] private List<Transform> _projectileSpawnPoints;
        [SerializeField] private List<Transform> _enemySpawnPoints;

        private EnemyMover _mover;
        private EnemyAnimator _view;
        private AttackerManager _attackerManager;
        private Transform _target;
        private List<AttackData> _attacksData;
        private AttackData _currentAttack;
        private AttackData _lastAttack;

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

            AttackData firstAttack = _attacksData[Random.Range(0, _attacksData.Count)];
            _attacksData.Remove(firstAttack);

            ReloadAllAttack();

            _currentAttack = firstAttack;
            _mover.SetAttackRange(firstAttack.AttackRange);
        }

        private void TryAttack(Transform target)
        {
            _target = target;

            if (_currentAttack != null)
            {
                _lastAttack = _currentAttack;
                _view.SetTriggerByName(_currentAttack.NameInAnimator);
                StartCoroutine(Reload(_currentAttack.Cooldown, _currentAttack));
            }
            else if (TryGetNewAttack(out AttackData newAttack))
            {
                _currentAttack = newAttack;
                _mover.SetAttackRange(_currentAttack.AttackRange);
            }
        }

        private void AttackToggle()
        {
            _attackerManager.ExecuteAttack(_lastAttack, this);
        }

        private void SetProjectileSpawnPointOnTarget() => _projectileSpawnPoints[0] = _target;

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

        private void ReloadAllAttack()
        {
            foreach (var attack in _attacksData)
                StartCoroutine(Reload(attack.Cooldown, attack));

            _attacksData.Clear();
        }

        private IEnumerator Reload(float cooldown, AttackData attack)
        {
            _currentAttack = null;

            yield return new WaitForSeconds(cooldown);

            _attacksData.Add(attack);
        }
    }
}