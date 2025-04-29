using Data.ExpPointsData;
using ExpPoints;
using Pools;
using UnityEngine;
using WalletSystem.MoneyLogic;
using Zenject;

namespace EnemyLogic
{
    public class EnemyEjector : MonoBehaviour
    {
        private readonly Vector3 _tolerance = new Vector2(0.1f, 0);
        private readonly Vector3 _offset = new Vector2(0, 0.5f);

        private MoneyPool _moneyPool;
        private int _money;
        private ExpPointPool _expPointPool;
        private ExpPointData _expPointData;

        [Inject]
        public void Construct(ExpPointPool expPointPool, MoneyPool moneyPool)
        {
            _expPointPool = expPointPool;
            _moneyPool = moneyPool;
        }

        public void Initialize(ExpPointData expPointData, int money)
        {
            _expPointData = expPointData;
            _money = money;
        }

        public void Eject()
        {
            ExpPoint point = _expPointPool.Get(_expPointData);
            point.transform.position = (transform.position + _tolerance) - _offset;

            Money money = _moneyPool.Get(_money);
            money.transform.position = (transform.position - _tolerance) - _offset;
        }
    }
}