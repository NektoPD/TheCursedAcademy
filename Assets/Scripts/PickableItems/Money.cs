using Pools;
using Pools.FromPrefab;
using UnityEngine;
using Utils;

namespace PickableItems
{
    public class Money : MonoBehaviour, IPoolEntity, IPickable
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
