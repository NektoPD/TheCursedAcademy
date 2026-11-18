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

        protected readonly List<T> EntityPool = new();
        private readonly HashSet<T> _inPoolSet = new();

        private readonly List<T> _allEntities = new();

        private int _count;

        public event Action Returned;

        protected Pool(DiContainer container)
        {
            Container = container;
        }

        public int Active => _count - EntityPool.Count;

        /// <summary>��������� ��� � ����: ��� ������ ��������� (� ����).</summary>
        public IReadOnlyCollection<T> Entites => EntityPool;

        /// <summary>��� ��������� ����� ������� (� ��������, � ���������).</summary>
        public IReadOnlyCollection<T> AllEntities => _allEntities;

        /// <summary>������ ��, ��� ������ �� � ���� (������/����������).</summary>
        public IEnumerable<T> SpawnedEntities
        {
            get
            {
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

            if (!_inPoolSet.Add(entity))
                return;

            EntityPool.Add(entity);
            Returned?.Invoke();
        }

        protected abstract T Initialize(IData<T> data, T entity);
        protected abstract T Create(IData<T> data);

        /// <summary>
        /// �����: ��� ������ ������ ������� entity �� EntityPool (��� � EnemyPool).
        /// </summary>
        protected abstract bool TryGetInPool(IData<T> data, out T entity);
    }
}
