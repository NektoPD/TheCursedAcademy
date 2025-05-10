using System;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers.TimeDatas
{
    [Serializable]
    public class GroupEnemysEventData : ITimeData
    {
        [SerializeField] private List<int> _ids; //õç
        [SerializeField] private int _count;
        [SerializeField] private int _startPlayTimeInSeconds;
        [SerializeField] private int _cooldown;

        public GroupEnemysEventData(int startPlayTimeInSeconds, List<int> ids, int count, int cooldown)
        {
            _ids = ids;
            _count = count;
            _startPlayTimeInSeconds = startPlayTimeInSeconds;
            _cooldown = cooldown;
        }

        public float PlayTimeInSeconds => _startPlayTimeInSeconds;

        public IReadOnlyList<int> EnemyIds => _ids;

        public int Count => _count;

        public int Cooldown => _cooldown;
    }
}