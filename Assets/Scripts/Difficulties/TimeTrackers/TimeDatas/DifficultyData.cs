using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers.TimeDatas
{
    [System.Serializable]
    public class DifficultyData : ITimeData
    {
        [SerializeField] private float _playTimeInSeconds;
        [SerializeField] private List<int> _enemyIds;

        public DifficultyData(float playTimeInSeconds, List<int> enemyIds)
        {
            _playTimeInSeconds = playTimeInSeconds;
            _enemyIds = enemyIds;
        }

        public float PlayTimeInSeconds => _playTimeInSeconds;

        public IReadOnlyList<int> EnemyIds => _enemyIds;
    }
}
