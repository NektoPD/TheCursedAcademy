using Difficulties.TimeTrackers.TimeDatas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Difficulties.TimeTrackers
{
    public abstract class TimeTracker<T> where T : ITimeData
    {
        protected readonly string Key;

        private readonly MonoBehaviour _monoBehaviour;

        private List<T> _timeDataList;

        public event Action<T> TimeComed;

        public TimeTracker(string key, string runnerName = "CoroutineRunner")
        {
            var go = new GameObject(runnerName);
            _monoBehaviour = go.AddComponent<CoroutineRunner>();
            Key = key;
        }

        public void Start()
        {
            if (TryLoadDifficultyData(out List<T> data))
            {
                _timeDataList = data.OrderBy(data => data.PlayTimeInSeconds).ToList();

                _monoBehaviour.StartCoroutine(TimeTrack());
            }
        }

        private IEnumerator TimeTrack()
        {
            if (_timeDataList == null)
                yield return null;

            while (_timeDataList.Count > 0)
            {
                T data = _timeDataList.First();

                _timeDataList.Remove(data);

                float cooldown = 0;

                if (Time.timeSinceLevelLoad >= data.PlayTimeInSeconds)
                {
                    TimeComed.Invoke(data);

                    if (_timeDataList.Count > 0)
                        cooldown = _timeDataList.First().PlayTimeInSeconds - Time.timeSinceLevelLoad;
                }
                else
                {
                    var temp = data.PlayTimeInSeconds - Time.timeSinceLevelLoad;
                    cooldown = Math.Clamp(temp, 0, data.PlayTimeInSeconds - temp);
                }

                yield return new WaitForSeconds(cooldown);
            }
        }

        protected abstract bool TryLoadDifficultyData(out List<T> data);
    }

    public class CoroutineRunner : MonoBehaviour { }
}