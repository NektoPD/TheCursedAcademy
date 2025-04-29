using WalletSystem.MoneyLogic;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Pools 
{
    public class MoneyPool
    {
        private readonly List<Money> _entityPool = new();
        protected readonly DiContainer Container;

        private Money _prefab;

        public MoneyPool(DiContainer container, Money prefab)
        {
            Container = container;
            _prefab = prefab;
        }

        public Money Get(int count)
        {
            if (_entityPool.Count > 0)
            {
                Money entity = _entityPool.First();
                entity.Initialize(count, this);
                _entityPool.Remove(entity);

                return entity;
            }

            var newEntity = Container.InstantiatePrefabForComponent<Money>(_prefab);
            newEntity.Initialize(count, this);
            return newEntity;
        }

        public void ReturnEntity(Money entity) => _entityPool.Add(entity);
    }
}
