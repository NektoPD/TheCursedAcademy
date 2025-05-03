using Data.ExpPointsData;
using EnemyLogic;
using UnityEngine;

namespace Training
{
    [RequireComponent(typeof(EnemyDamageTaker), typeof(EnemyEjector))]
    public class EnemyInitializer : MonoBehaviour
    {
        [SerializeField] private float _maxHp;
        [SerializeField] private float _immuneTime;
        [SerializeField] private ExpPointData _expPoint;
        [SerializeField] private int _money;

        private EnemyDamageTaker _damageTracker;
        private EnemyEjector _enemyEjector;

        private void Awake()
        {
            _damageTracker = GetComponent<EnemyDamageTaker>();
            _enemyEjector = GetComponent<EnemyEjector>();
        }

        private void Start()
        {
            _damageTracker.Initialize(_maxHp, _immuneTime);
            _enemyEjector.Initialize(_expPoint, _money);
        }
    }
}