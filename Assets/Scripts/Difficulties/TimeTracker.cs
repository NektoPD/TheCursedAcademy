using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Difficulties
{
    public class TimeTracker : MonoBehaviour
    {
        private const string Key = "DifficultyData";

        private List<DifficultyData> _difficultyDataList;
        private Coroutine _coroutine;

        public event Action<List<int>> TimeComed;

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private void Start()
        {
            if (TryLoadDifficultyData(out List<DifficultyData> data))
            {
                _difficultyDataList = data;
                _coroutine = StartCoroutine(TimeTrack());
            }
        }

        private bool TryLoadDifficultyData(out List<DifficultyData> data)
        {
            data = null;

            if (PlayerPrefs.HasKey(Key))
            {
                string json = PlayerPrefs.GetString(Key);

                data = JsonConvert.DeserializeObject<List<DifficultyData>>(json);
            }

            return data != null;
        }

        private IEnumerator TimeTrack()
        {
            while (_difficultyDataList.Count > 0)
            {
                DifficultyData data = _difficultyDataList.First();
                _difficultyDataList.Remove(data);

                float cooldown = 0;

                if (Time.timeSinceLevelLoad >= data.PlayTime)
                {
                    TimeComed?.Invoke(data.EnemyIds.ToList());

                    if (_difficultyDataList.Count > 0)
                        cooldown = _difficultyDataList.First().PlayTime - Time.timeSinceLevelLoad;
                }
                else
                {
                    var temp = data.PlayTime - Time.timeSinceLevelLoad;
                    cooldown = Math.Clamp(temp, 0, data.PlayTime - temp);
                }

                yield return new WaitForSeconds(cooldown);
            }
        }
    }
}