using System;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers.TimeDatas
{
    [Serializable]
    public class GroupEnemysEventData : ITimeData
    {
        [SerializeField] private List<int> _enemyIds;
        [SerializeField] private int _count;
        [SerializeField] private float _playTimeInSeconds;
        [SerializeField] private int _cooldown;

        public GroupEnemysEventData(float playTimeInSeconds, List<int> enemyIds, int count, int cooldown)
        {
            _enemyIds = enemyIds;
            _count = count;
            _playTimeInSeconds = playTimeInSeconds;
            _cooldown = cooldown;
        }

        public float PlayTimeInSeconds => _playTimeInSeconds;

        public IReadOnlyList<int> EnemyIds => _enemyIds;

        public int Count => _count;

        public int Cooldown => _cooldown;
    }
}