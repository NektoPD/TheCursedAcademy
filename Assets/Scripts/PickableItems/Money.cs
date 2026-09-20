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
        private BoxCollider2D _collider;
        private Vector2 _baseColliderSize;

        public int Value => _value;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _baseColliderSize = _collider.size;
        }

        public void Initialize(int count, MoneyPool pool)
        {
            _pool = pool;
            _value = count;
            _collider.size = _baseColliderSize * PickupRadius.Scale;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}
