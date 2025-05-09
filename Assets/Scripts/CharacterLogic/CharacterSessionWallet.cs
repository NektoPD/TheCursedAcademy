using System;

namespace CharacterLogic
{
    public class CharacterSessionWallet : IDisposable
    {
        private CharacterCollisionHandler _characterCollisionHandler;
        private bool _disposed = false;

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

            CollectedMoney += value;
        }

        public void ClearWallet()
        {
            CollectedMoney = 0;
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
                    _characterCollisionHandler.GotMoney -= AddMoney;
                    _characterCollisionHandler = null;
                }

                _disposed = true;
            }
        }
    }
}