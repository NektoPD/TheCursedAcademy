using System;
using UnityEngine;

namespace CharacterLogic
{
    public class CharacterLevelController : IDisposable
    {
        private const int InitialExperienceRequirement = 8;
        private const double ExpGrowthFactor = 1.5;

        private int _experienceIncreaseValue = 10;
        private float _expMultiplier = 1f;
        private int _currentLevel = 1;
        private int _requiredExpForNextLevel = InitialExperienceRequirement;
        private CharacterCollisionHandler _characterCollisionHandler;
        private bool _disposed = false;

        public int CurrentExp { get; private set; } = 0;
        public int CurrentLevel => _currentLevel;
        public int RequiredExpForNextLevel => _requiredExpForNextLevel;

        public event Action LeveledUp;

        public void IncreaseExp(int value)
        {
            CurrentExp += Mathf.Max(1, Mathf.RoundToInt(value * _expMultiplier));

            while (CurrentExp >= _requiredExpForNextLevel)
            {
                LevelUp();
            }
        }

        public void Initialize(CharacterCollisionHandler characterCollisionHandler)
        {
            _characterCollisionHandler = characterCollisionHandler;
            _characterCollisionHandler.GotExpPoint += IncreaseExp;
        }

        private void LevelUp()
        {
            _currentLevel++;

            CurrentExp -= _requiredExpForNextLevel;
            _experienceIncreaseValue = (int)(_experienceIncreaseValue * ExpGrowthFactor);
            _requiredExpForNextLevel =
                (int)(InitialExperienceRequirement * Math.Pow(ExpGrowthFactor, _currentLevel - 1));

            LeveledUp?.Invoke();
        }

        public void ClearExperience()
        {
            CurrentExp = 0;
        }

        public int GetExperienceIncreaseValue()
        {
            return _experienceIncreaseValue;
        }

        public void SetExpMultiplier(float multiplier)
        {
            _expMultiplier = Mathf.Max(1f, multiplier);
        }

        public void ResetCharacter()
        {
            _currentLevel = 1;
            CurrentExp = 0;
            _experienceIncreaseValue = 10;
            _requiredExpForNextLevel = InitialExperienceRequirement;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _characterCollisionHandler != null)
                {
                    _characterCollisionHandler.GotExpPoint -= IncreaseExp;
                    _characterCollisionHandler = null;
                }

                _disposed = true;
            }
        }
    }
}