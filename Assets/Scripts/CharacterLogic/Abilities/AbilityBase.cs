using System;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public abstract class AbilityBase : MonoBehaviour
    {
        protected AbilityConfig Config;
        protected Transform OwnerTransform;

        private float _currentCharge;
        private bool _isReady;

        public bool IsReady => _isReady;
        public bool IsActive { get; protected set; }

        public event Action AbilityReady;
        public event Action<float, float> ChargeChanged;

        public virtual void Initialize(AbilityConfig config, Transform ownerTransform)
        {
            Config = config;
            OwnerTransform = ownerTransform;
            _currentCharge = 0f;
            _isReady = false;
            IsActive = false;
        }

        public void AddCharge(int amount = 1)
        {
            if (_isReady || IsActive) return;

            _currentCharge += amount;
            ChargeChanged?.Invoke(_currentCharge, Config.KillsToCharge);

            if (_currentCharge >= Config.KillsToCharge)
            {
                _isReady = true;
                AbilityReady?.Invoke();
            }
        }

        public void Activate()
        {
            if (!_isReady || IsActive) return;

            _isReady = false;
            _currentCharge = 0f;
            ChargeChanged?.Invoke(_currentCharge, Config.KillsToCharge);
            Execute();
        }

        protected abstract void Execute();
    }
}
