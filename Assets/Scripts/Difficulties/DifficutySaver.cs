using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties
{
    public class DifficutySaver : MonoBehaviour
    {
        private const string Key = "DifficultyData";

        private void Start()
        {
            SaveDifficultyData();
        }

        private void SaveDifficultyData()
        {
            List<DifficultyData> dataList = new()
            {
                new DifficultyData(0f, new List<int> { 1, 2, 3 }),
                new DifficultyData(2f, new List<int> { 4, 5, 6 }),
            };

            string json = JsonConvert.SerializeObject(dataList);

            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }
    }
}