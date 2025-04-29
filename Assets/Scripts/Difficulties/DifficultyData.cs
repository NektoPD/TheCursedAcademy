using System.Collections.Generic;
using UnityEngine;

namespace Difficulties
{
    [System.Serializable]
    public class DifficultyData
    {
        [SerializeField] private float _playTime;
        [SerializeField] private List<int> _enemyIds;

        public DifficultyData(float playTime, List<int> enemyIds)
        {
            _playTime = playTime;
            _enemyIds = enemyIds;
        }

        public float PlayTime => _playTime;

        public IReadOnlyList<int> EnemyIds => _enemyIds;
    }
}
