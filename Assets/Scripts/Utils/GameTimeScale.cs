using UnityEngine;

namespace Utils
{
    public static class GameTimeScale
    {
        public static bool IsPauseActive { get; private set; }

        public static void SetPauseActive(bool isActive)
        {
            IsPauseActive = isActive;
            if (isActive)
                Time.timeScale = 0f;
        }

        public static void Set(float value)
        {
            Time.timeScale = IsPauseActive ? 0f : value;
        }

        public static void ForcePause()
        {
            Time.timeScale = 0f;
        }
    }
}
