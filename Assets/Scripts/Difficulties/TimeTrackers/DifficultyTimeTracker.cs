using Difficulties.TimeTrackers.TimeDatas;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Difficulties.TimeTrackers
{
    public class DifficultyTimeTracker : TimeTracker<DifficultyData>
    {
        public DifficultyTimeTracker(string key = nameof(DifficultyData)) : base(key) { }

        protected override bool TryLoadDifficultyData(out List<DifficultyData> data)
        {
            data = null;

            if (PlayerPrefs.HasKey(Key))
            {
                string json = PlayerPrefs.GetString(Key);

                data = JsonConvert.DeserializeObject<List<DifficultyData>>(json);
            }

            return data != null;
        }
    }
}