using Data;
using EnemyLogic;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Pools
{
    public class EnemyPool : Pool<Enemy>
    {
        public event Action<Transform> EnemyReturned;

        public EnemyPool(DiContainer container) : base(container) { }

        protected override Enemy Create(IData<Enemy> data) => Container.InstantiatePrefabForComponent<Enemy>(data.Prefab);

        protected override Enemy Initialize(IData<Enemy> data, Enemy entity)
        {
            entity.ResetEntity();
            entity.Initialize(data, this);
            return entity;
        }

        protected override bool TryGetInPool(IData<Enemy> data, out Enemy entity)
        {
            entity = null;

            var currentEntity = EntityPool.Where(entity => entity.Prefab == data.Prefab);

            if (currentEntity.Count() > 0)
            {
                entity = currentEntity.First();
                entity = Initialize(data, entity);
                EntityPool.Remove(entity);

                return true;
            }

            return false;
        }

        public override void ReturnEntity(Enemy entity)
        {
            base.ReturnEntity(entity);
            EnemyReturned?.Invoke(entity.transform);
        }
    }
}
