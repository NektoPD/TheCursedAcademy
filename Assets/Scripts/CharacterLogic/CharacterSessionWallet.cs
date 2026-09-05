using System;
using UnityEngine;

namespace CharacterLogic
{
    public class CharacterSessionWallet : IDisposable
    {
        private CharacterCollisionHandler _characterCollisionHandler;
        private bool _disposed;
        private float _multiplier = 1f;
        private float _fractionalMoney;

        public int CollectedMoney { get; private set; }

        public void Initialize(CharacterCollisionHandler characterCollisionHandler)
        {
            _characterCollisionHandler = characterCollisionHandler;
            _characterCollisionHandler.GotMoney += AddMoney;
        }

        public void AddMoney(int value)
        {
            if (value <= 0)
                return;

            float rewardedMoney = value * _multiplier + _fractionalMoney;
            int wholeMoney = Mathf.FloorToInt(rewardedMoney);
            _fractionalMoney = rewardedMoney - wholeMoney;
            CollectedMoney += wholeMoney;
        }

        public void SetMultiplier(float multiplier)
        {
            _multiplier = Mathf.Max(1f, multiplier);
        }

        public void ClearWallet()
        {
            CollectedMoney = 0;
            _fractionalMoney = 0f;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _characterCollisionHandler != null)
            {
                _characterCollisionHandler.GotMoney -= AddMoney;
                _characterCollisionHandler = null;
            }

            _disposed = true;
        }
    }
}
