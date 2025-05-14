using Pools;
using Pools.FromPrefab;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Zenject;

namespace PickableItems
{
    public class Magnet : MonoBehaviour, IPoolEntity, IPickable
    {
        private Dictionary<IPickable, Transform> _pickables;
        private MagnetPool _pool;
        private MoneyPool _moneyPool;
        private ExpPointPool _expPointPool;
        private HealPool _healPool;
        private int _value;

        public int Value => _value;

        [Inject]
        private void Construct(MoneyPool moneyPool, ExpPointPool expPointPool, HealPool healPool)
        {
            _moneyPool = moneyPool;
            _expPointPool = expPointPool;
            _healPool = healPool;
        }

        public void Initialize(int count, MagnetPool pool)
        {
            _pool = pool;
            _value = count;
        }

        public Dictionary<IPickable, Transform> GetAllActivePickableItems()
        {
            _pickables = new();

            foreach (var item in _moneyPool.Entites.Where(money => money.isActiveAndEnabled))
                _pickables.Add(item, item.transform);

            foreach (var item in _expPointPool.Entites.Where(expPoint => expPoint.isActiveAndEnabled))
                _pickables.Add(item, item.transform);

            foreach (var item in _healPool.Entites.Where(heal => heal.isActiveAndEnabled))
                _pickables.Add(item, item.transform);

            return _pickables;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}