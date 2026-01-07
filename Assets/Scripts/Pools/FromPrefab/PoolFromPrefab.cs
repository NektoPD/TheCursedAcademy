using System;
using System.Collections.Generic;
using Zenject;

namespace Pools.FromPrefab
{
    public abstract class PoolFromPrefab<T> where T : IPoolEntity
    {
        // FREE
        private readonly List<T> _entityPool = new();
        private readonly HashSet<T> _inPool = new();

        // ALL
        private readonly List<T> _allEntities = new();

        protected readonly DiContainer Container;
        private readonly T _prefab;

        public event Action EntityReturned;

        protected PoolFromPrefab(DiContainer container, T prefab)
        {
            Container = container;
            _prefab = prefab;
        }

        /// <summary>Свободные (в пуле)</summary>
        public IReadOnlyCollection<T> Entites => _entityPool;

        /// <summary>Заспавненные (не в пуле)</summary>
        public IEnumerable<T> SpawnedEntities
        {
            get
            {
                for (int i = 0; i < _allEntities.Count; i++)
                {
                    var e = _allEntities[i];
                    if (!_inPool.Contains(e))
                        yield return e;
                }
            }
        }

        public T Get(int count)
        {
            T entity;

            if (_entityPool.Count > 0)
            {
                // Берём с конца (O(1)) вместо First()/Remove()
                int lastIndex = _entityPool.Count - 1;
                entity = _entityPool[lastIndex];
                _entityPool.RemoveAt(lastIndex);
                _inPool.Remove(entity);
            }
            else
            {
                entity = Instantiate(_prefab);
                _allEntities.Add(entity);
            }

            Initialize(entity, count);
            return entity;
        }

        public void ReturnEntity(T entity)
        {
            EntityReturned?.Invoke();

            // Защита от double-return
            if (!_inPool.Add(entity))
                return;

            _entityPool.Add(entity);
        }

        protected abstract void Initialize(T entity, int count);
        protected abstract T Instantiate(T prefab);
    }
}
