using Difficulties.TimeTrackers.TimeDatas;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers
{
    public class EventTimeTracker : TimeTracker<GroupEnemysEventData>
    {
        public EventTimeTracker(string key = nameof(GroupEnemysEventData)) : base(key) { }

        protected override bool TryLoadDifficultyData(out List<GroupEnemysEventData> data)
        {
            data = null;

            if (PlayerPrefs.HasKey(Key))
            {
                string json = PlayerPrefs.GetString(Key);
                Debug.Log(json);
                data = JsonConvert.DeserializeObject<List<GroupEnemysEventData>>(json);
                Debug.Log(data[0].EnemyIds.Count);
            }

            return data != null;
        }
    }
}