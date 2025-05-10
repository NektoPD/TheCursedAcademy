using System;
using TMPro;
using UnityEngine;

namespace UI 
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeView : MonoBehaviour
    {
        private TextMeshProUGUI _timeText;

        private void Awake()
        {
            _timeText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);

            string formattedTime;

            if (timeSpan.TotalHours >= 1) 
                formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);         
            else
                formattedTime = string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            _timeText.text = formattedTime;
        }
    }
}