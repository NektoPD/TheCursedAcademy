using System;
using HealthSystem;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Animator))]
    public class DummyTutorial : MonoBehaviour, IDamageable
    {
        private const string HitTrigger = "Hit";

        [SerializeField] private int _hitsToComplete = 5;

        private Animator _animator;
        private int _hitCount;
        private bool _completed;

        public bool IsDied => false;

        public event Action HitsCompleted;
        public event Action<int, int> HitRegistered;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            _animator.SetTrigger(HitTrigger);

            if (_completed)
                return;

            _hitCount++;
            HitRegistered?.Invoke(_hitCount, _hitsToComplete);

            if (_hitCount >= _hitsToComplete)
            {
                _completed = true;
                HitsCompleted?.Invoke();
            }
        }
    }
}
