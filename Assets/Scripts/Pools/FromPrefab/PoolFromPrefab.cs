using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Pools.FromPrefab
{
    public abstract class PoolFromPrefab<T> where T : IPoolEntity
    {
        private readonly List<T> _entityPool = new();
        protected readonly DiContainer Container;
        private readonly T _prefab;

        public event Action EntityReturned;

        public PoolFromPrefab(DiContainer container, T prefab)
        {
            Container = container;
            _prefab = prefab;
        }

        public IReadOnlyCollection<T> Entites => _entityPool;

        public T Get(int count)
        {
            if (_entityPool.Count > 0)
            {
                T entity = _entityPool.First();
                Initialize(entity, count);
                _entityPool.Remove(entity);

                return entity;
            }

            var newEntity = Instantiate(_prefab);
            Initialize(newEntity, count);
            return newEntity;
        }

        public void ReturnEntity(T entity)
        {
            EntityReturned?.Invoke();
            _entityPool.Add(entity);
        }

        protected abstract void Initialize(T entity, int count);

        protected abstract T Instantiate(T prefab);
    }
}