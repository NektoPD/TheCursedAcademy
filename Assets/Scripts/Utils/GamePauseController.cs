using UnityEngine;
using UnityEngine.EventSystems;
using YG;

namespace Utils
{
    public sealed class GamePauseController : MonoBehaviour
    {
        private static GamePauseController _instance;
        private static int _pauseRequests;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (_instance != null)
                return;

            GameObject gameObject = new GameObject(nameof(GamePauseController));
            DontDestroyOnLoad(gameObject);
            _instance = gameObject.AddComponent<GamePauseController>();
        }

        public static void HoldPause()
        {
            EnsureCreated();
            _pauseRequests++;
            ApplyPause();
        }

        public static void ReleasePause()
        {
            if (_pauseRequests > 0)
                _pauseRequests--;

            if (_pauseRequests == 0)
                YG2.PauseGame(false);
        }

        private static void EnsureCreated()
        {
            if (_instance == null)
                Create();
        }

        private static void ApplyPause()
        {
            Time.timeScale = 0f;
            YG2.PauseGameNoEditEventSystem(true);
        }

        private void OnEnable()
        {
            YG2.onFocusWindowGame += OnFocusWindowGame;
            YG2.onPauseGame += OnPauseGame;
        }

        private void OnDisable()
        {
            YG2.onFocusWindowGame -= OnFocusWindowGame;
            YG2.onPauseGame -= OnPauseGame;
        }

        private void Update()
        {
            if (_pauseRequests > 0)
                ApplyPause();
        }

        private void LateUpdate()
        {
            if (_pauseRequests <= 0)
                return;

            foreach (EventSystem eventSystem in FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                eventSystem.enabled = true;
        }

        private void OnFocusWindowGame(bool isFocused)
        {
            if (_pauseRequests > 0 && isFocused)
                ApplyPause();
        }

        private void OnPauseGame(bool isPaused)
        {
            if (_pauseRequests > 0 && isPaused == false)
                ApplyPause();
        }
    }
}
