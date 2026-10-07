using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeView : MonoBehaviour
    {
        [SerializeField] private bool _waitForGameStart = true;

        private TextMeshProUGUI _timeText;
        private bool _isRunning;
        private float _elapsed;

        private void Awake()
        {
            _timeText = GetComponent<TextMeshProUGUI>();
            _isRunning = !_waitForGameStart;
            UpdateText();
        }

        public void StartTimer()
        {
            if (_isRunning)
                return;

            _elapsed = 0f;
            _isRunning = true;
        }

        private void LateUpdate()
        {
            if (!_isRunning)
                return;

            _elapsed += Time.deltaTime;
            UpdateText();
        }

        private void UpdateText()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_elapsed);

            string formattedTime;

            if (timeSpan.TotalHours >= 1)
                formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes,
                    timeSpan.Seconds);
            else
                formattedTime = string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            _timeText.text = formattedTime;
        }
    }
}