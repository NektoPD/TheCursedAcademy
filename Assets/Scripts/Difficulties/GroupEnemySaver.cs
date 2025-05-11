using Difficulties.TimeTrackers.TimeDatas;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties
{
    public class GroupEnemySaver : MonoBehaviour
    {
        private const string DataKey = nameof(GroupEnemysEventData);

        [SerializeField] private List<GroupEnemysEventData> _dataList;

        private void Start()
        {
            string json = JsonConvert.SerializeObject(_dataList);

            PlayerPrefs.SetString(DataKey, json);
            PlayerPrefs.Save();
        }
    }
}