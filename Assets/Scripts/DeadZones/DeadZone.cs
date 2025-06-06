using EnemyLogic;
using Pools;
using UnityEngine;

namespace DeadZones
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeadZone : MonoBehaviour
    {
        [SerializeField] private Enemy _bossPrefab;

        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Enemy enemy))
                if (enemy.Prefab == _bossPrefab)
                    return;

            if (other.TryGetComponent(out IPoolEntity entity))
                entity.Despawn();
        }

        public void SetupZone(Vector2 size, Vector3 position)
        {
            _collider.size = size;
            _collider.isTrigger = true;

            transform.position = position;
        }
    }
}

