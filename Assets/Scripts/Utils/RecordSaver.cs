using UnityEngine;
using YG;

namespace Utils
{
    public class RecordSaver : MonoBehaviour
    {
        [SerializeField] private string _leaderboard = "mainLeaderboard";

        private string PrefKey => $"LB_RECORD_SECONDS_{_leaderboard}";

        private void OnEnable()
        {
            YG2.onGetSDKData += TryUploadSavedRecord;
            TryUploadSavedRecord();
        }

        private void OnDisable()
        {
            SaveRecordNow();
            YG2.onGetSDKData -= TryUploadSavedRecord;
        }

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

            if (current > saved)
            {
                PlayerPrefs.SetFloat(PrefKey, current);
                PlayerPrefs.Save();
            }

            TryUploadSavedRecord();
        }

        private void TryUploadSavedRecord()
        {
            if (!YG2.isSDKEnabled || !YG2.player.auth)
                return;

            float saved = GetSavedRecordSeconds();

            if (saved > 0f)
                YG2.SetLBTimeConvert(_leaderboard, saved);
        }
    }
}
