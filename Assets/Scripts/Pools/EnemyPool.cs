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
            entity = EntityPool.FirstOrDefault(e => e.Prefab == data.Prefab);
            if (entity == null)
                return false;

            EntityPool.Remove(entity);
            return true;
        }

        public override void ReturnEntity(Enemy entity)
        {
            base.ReturnEntity(entity);
            EnemyReturned?.Invoke(entity.transform);
        }
    }
}
