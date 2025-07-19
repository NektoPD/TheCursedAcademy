namespace EnemyLogic
{
    public class KilledEnemyCounter
    {
        public int KilledCounter { get; private set; }

        public void ResetCounter()
        {
            KilledCounter = 0;
        }

        public void AddKilledEnemy()
        {
            KilledCounter++;
        }
    }
}