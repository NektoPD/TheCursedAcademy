using Pools;
using Pools.FromPrefab;
using UnityEngine;
using Utils;

namespace PickableItems
{
    public class Heal : MonoBehaviour, IPoolEntity, IPickable
    {
        private HealPool _pool;
        private int _value;

        public int Value => _value;

        public void Initialize(int count, HealPool pool)
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