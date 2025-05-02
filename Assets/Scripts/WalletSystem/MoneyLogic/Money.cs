using Pools;
using UnityEngine;

namespace WalletSystem.MoneyLogic
{
    public class Money : MonoBehaviour, IPoolEntity, IMoney
    {
        private MoneyPool _pool;
        private int _value = 0;

        public int Value => _value;

        public void Initialize(int count, MoneyPool pool)
        {
            _pool = pool;
            _value = count;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}
