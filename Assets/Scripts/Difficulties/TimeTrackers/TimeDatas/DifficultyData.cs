using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers.TimeDatas
{
    [System.Serializable]
    public class DifficultyData : ITimeData
    {
        [SerializeField] private int _playTimeInSeconds;
        [SerializeField] private List<int> _enemyIds;

        public DifficultyData(int playTime, List<int> enemyIds)
        {
            _playTimeInSeconds = playTime;
            _enemyIds = enemyIds;
        }

        public float PlayTimeInSeconds => _playTimeInSeconds;

        public IReadOnlyList<int> EnemyIds => _enemyIds;
    }
}
