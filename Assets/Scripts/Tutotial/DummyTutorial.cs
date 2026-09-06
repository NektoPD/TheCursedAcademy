using System;
using HealthSystem;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Collider2D), typeof(Animator))]
    public class DummyTutorial : MonoBehaviour, IDamageable
    {
        private static readonly int HitTrigger = Animator.StringToHash("Hit");

        [SerializeField, Min(1)] private int _hitsToComplete = 5;

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
    }
}
