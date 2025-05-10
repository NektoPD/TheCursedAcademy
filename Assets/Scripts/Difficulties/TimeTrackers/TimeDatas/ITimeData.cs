using System.Collections.Generic;

namespace Difficulties.TimeTrackers.TimeDatas
{
    public interface ITimeData
    {
        public float PlayTimeInSeconds { get; }

        public IReadOnlyList<int> EnemyIds {get; }
    }
}