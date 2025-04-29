using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties
{
    public class DifficutySaver : MonoBehaviour
    {
        private const string DataKey = "DifficultyData";
        private const string CooldownKey = "DifficultyCooldown";
        private const string MaxEnemyKey = "DifficultyMaxEnemy";

        [SerializeField] private float _cooldown = 1;
        [SerializeField] private int _maxEnemy = 100;
        [SerializeField] private List<DifficultyData> _dataList;

        private void Start()
        {
            SaveDifficultyData();
        }

        private void SaveDifficultyData()
        {
            string json = JsonConvert.SerializeObject(_dataList);

            PlayerPrefs.SetString(DataKey, json);
            PlayerPrefs.SetFloat(CooldownKey, _cooldown);
            PlayerPrefs.SetInt(MaxEnemyKey, _maxEnemy);
            PlayerPrefs.Save();
        }
    }
}