using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pools
{
    public abstract class Pool<T> where T : Component, IPoolEntity
    {
        protected readonly DiContainer Container;

        // FREE (в пуле)
        protected readonly List<T> EntityPool = new();
        private readonly HashSet<T> _inPoolSet = new();

        // ALL (все созданные этим пулом)
        private readonly List<T> _allEntities = new();

        private int _count;

        public event Action Returned;

        protected Pool(DiContainer container)
        {
            Container = container;
        }

        public int Active => _count - EntityPool.Count;

        /// <summary>Оставляем как у тебя: это именно свободные (в пуле).</summary>
        public IReadOnlyCollection<T> Entites => EntityPool;

        /// <summary>Все созданные пулом объекты (и активные, и свободные).</summary>
        public IReadOnlyCollection<T> AllEntities => _allEntities;

        /// <summary>Только те, кто сейчас НЕ в пуле (выданы/заспавнены).</summary>
        public IEnumerable<T> SpawnedEntities
        {
            get
            {
                // выдаём только тех, кто не в пуле
                for (int i = 0; i < _allEntities.Count; i++)
                {
                    var e = _allEntities[i];
                    if (!_inPoolSet.Contains(e))
                        yield return e;
                }
            }
        }

        public T Get(IData<T> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Prefab == null) throw new ArgumentException("data.Prefab is null", nameof(data));

            if (TryGetInPool(data, out T entity))
            {
                // TryGetInPool должен сделать EntityPool.Remove(entity)
                _inPoolSet.Remove(entity);
                return Initialize(data, entity);
            }

            _count++;
            var newEntity = Create(data);
            if (newEntity == null) throw new InvalidOperationException($"{GetType().Name}.Create returned null");

            _allEntities.Add(newEntity);
            return Initialize(data, newEntity);
        }

        public virtual void ReturnEntity(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // защита от double-return
            if (!_inPoolSet.Add(entity))
                return;

            EntityPool.Add(entity);
            Returned?.Invoke();
        }

        protected abstract T Initialize(IData<T> data, T entity);
        protected abstract T Create(IData<T> data);

        /// <summary>
        /// ВАЖНО: при успехе должен удалить entity из EntityPool (как в EnemyPool).
        /// </summary>
        protected abstract bool TryGetInPool(IData<T> data, out T entity);
    }
}
