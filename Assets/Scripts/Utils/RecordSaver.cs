using UnityEngine;
using YG;

namespace Utils
{
    public class RecordSaver : MonoBehaviour
    {
        [SerializeField] private string _leaderboard = "mainLeaderboard";

        private string PrefKey => $"LB_RECORD_SECONDS_{_leaderboard}";

        public float GetCurrentRunSeconds()
        {
            return Time.timeSinceLevelLoad;
        }

        public float GetSavedRecordSeconds()
        {
            return PlayerPrefs.GetFloat(PrefKey, 0f);
        }

        public void SaveRecordNow()
        {
            float current = GetCurrentRunSeconds();

            float saved = GetSavedRecordSeconds();

            if (current <= saved)
                return;

            PlayerPrefs.SetFloat(PrefKey, current);
            PlayerPrefs.Save();

            YG2.SetLBTimeConvert(_leaderboard, current);
        }

        private void OnDisable()
        {
            SaveRecordNow();
        }

        private void OnDestroy()
        {
            SaveRecordNow();
        }
    }
}