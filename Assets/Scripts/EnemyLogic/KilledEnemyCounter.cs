using System;

namespace EnemyLogic
{
    public class KilledEnemyCounter
    {
        public int KilledCounter { get; private set; }

        public event Action EnemyKilled;

        public void ResetCounter()
        {
            KilledCounter = 0;
        }

        public void AddKilledEnemy()
        {
            KilledCounter++;
            EnemyKilled?.Invoke();
        }
    }
}