using System;
using UnityEngine;
using Utils;

namespace CharacterLogic.Abilities
{
    public abstract class AbilityBase : MonoBehaviour
    {
        protected AbilityConfig Config;
        protected Transform OwnerTransform;
        protected SimpleSpriteAnimator ActivationEffect;
        private CharacterSoundController _soundController;

        private float _currentCharge;
        private bool _isReady;

        public bool IsReady => _isReady;
        public bool IsActive { get; protected set; }

        public event Action AbilityReady;
        public event Action<float, float> ChargeChanged;

        public virtual void Initialize(AbilityConfig config, Transform ownerTransform, CharacterSoundController soundController)
        {
            Config = config;
            OwnerTransform = ownerTransform;
            _soundController = soundController;
            _currentCharge = 0f;
            _isReady = false;
            IsActive = false;
        }

        public void SetActivationEffect(SimpleSpriteAnimator effect)
        {
            ActivationEffect = effect;
        }

        public void FillCharge()
        {
            if (_isReady || IsActive) return;

            AddCharge(Config.KillsToCharge);
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

            if (_soundController != null)
                _soundController.EnableSoundByType(Config.ActivationSound);

            Execute();
        }

        protected abstract void Execute();
    }
}
