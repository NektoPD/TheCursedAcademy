using Data;
using System;
using System.Collections.Generic;
using Zenject;

namespace Pools
{
    public abstract class Pool<T> where T : IPoolEntity
    {
        protected readonly DiContainer Container;
        protected readonly List<T> EntityPool = new();

        private int _count;

        public event Action Returned;

        public Pool(DiContainer container)
        {
            Container = container;
        }

        public int Active => _count - EntityPool.Count;

        public IReadOnlyCollection<T> Entites => EntityPool;
        public T Get(IData<T> data)
        {
            if (TryGetInPool(data, out T entity))
            {
                entity = Initialize(data, entity);
                EntityPool.Remove(entity);

                return entity;
            }

            _count++;

            var newEntity = Create(data);
            newEntity = Initialize(data, newEntity);
            return newEntity;
        }

        public virtual void ReturnEntity(T entity)
        {
            EntityPool.Add(entity);
            Returned?.Invoke();
        }

        protected abstract T Initialize(IData<T> data, T entity);

        protected abstract T Create(IData<T> data);

        protected abstract bool TryGetInPool(IData<T> data, out T entity);
    }
}
