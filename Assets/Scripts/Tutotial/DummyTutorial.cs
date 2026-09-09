using System;
using Data.ExpPointsData;
using HealthSystem;
using PickableItems;
using Pools;
using Pools.FromPrefab;
using UnityEngine;
using Zenject;

namespace Tutorial
{
    [RequireComponent(typeof(Collider2D), typeof(Animator))]
    public class DummyTutorial : MonoBehaviour, IDamageable
    {
        private static readonly int HitTrigger = Animator.StringToHash("Hit");

        [SerializeField, Min(1)] private int _hitsToComplete = 5;
        [SerializeField] private ExpPointData _rewardExperience;
        [SerializeField, Min(1)] private int _rewardMoney = 5;
        [SerializeField] private Vector2 _experienceDropOffset = new(-0.35f, -0.5f);
        [SerializeField] private Vector2 _moneyDropOffset = new(0.35f, -0.5f);

        private Animator _animator;
        private ExpPointPool _expPointPool;
        private MoneyPool _moneyPool;
        private int _hitCount;
        private bool _completed;
        private bool _rewardsEjected;

        public bool IsDied => false;

        public event Action HitsCompleted;
        public event Action<int, int> HitRegistered;

        [Inject]
        private void Construct(ExpPointPool expPointPool, MoneyPool moneyPool)
        {
            _expPointPool = expPointPool;
            _moneyPool = moneyPool;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage, bool isFromBerserk = false)
        {
            _animator.SetTrigger(HitTrigger);

            if (_completed)
                return;

            _hitCount++;
            HitRegistered?.Invoke(_hitCount, _hitsToComplete);

            if (_hitCount < _hitsToComplete)
                return;

            _completed = true;
            HitsCompleted?.Invoke();
        }

        public void EjectRewards()
        {
            if (_rewardsEjected)
                return;

            _rewardsEjected = true;

            if (_rewardExperience != null && _expPointPool != null)
            {
                ExpPoint experience = _expPointPool.Get(_rewardExperience);
                experience.transform.position = transform.position + (Vector3)_experienceDropOffset;
            }

            if (_moneyPool != null)
            {
                Money money = _moneyPool.Get(_rewardMoney);
                money.transform.position = transform.position + (Vector3)_moneyDropOffset;
            }
        }
    }
}
