using UnityEngine;
using UnityEngine.EventSystems;
using YG;

namespace Utils
{
    public sealed class GamePauseController : MonoBehaviour
    {
        private static GamePauseController _instance;
        private static bool _isPauseHeld;
        private static bool _muteAudio;

        private static bool IsPauseHeld => _isPauseHeld || GameTimeScale.IsPauseActive;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (_instance != null)
                return;

            GameObject gameObject = new GameObject(nameof(GamePauseController));
            DontDestroyOnLoad(gameObject);
            _instance = gameObject.AddComponent<GamePauseController>();
        }

        public static void HoldPause(bool muteAudio = true)
        {
            EnsureCreated();
            _isPauseHeld = true;
            _muteAudio = muteAudio;
            ApplyPause();
        }

        public static void ReleasePause()
        {
            _isPauseHeld = false;

            YG2.PauseGame(false);
            GameTimeScale.Set(1f);
        }

        private static void EnsureCreated()
        {
            if (_instance == null)
                Create();
        }

        private static void ApplyPause()
        {
            Time.timeScale = 0f;

            if (!_isPauseHeld)
                return;

            YG2.PauseGame(true, editTimeScale: true, editAudioPause: _muteAudio,
                editCursor: true, editEventSystem: false);
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
            if (IsPauseHeld)
                ApplyPause();
            else if (YG2.isPauseGame && YG2.isFocusWindowGame && !YG2.nowAdsShow)
                YG2.PauseGame(false);
        }

        private void LateUpdate()
        {
            if (!IsPauseHeld)
                return;

            ApplyPause();

            foreach (EventSystem eventSystem in FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                eventSystem.enabled = true;
        }

        private void OnFocusWindowGame(bool isFocused)
        {
            if (IsPauseHeld && isFocused)
                ApplyPause();
        }

        private void OnPauseGame(bool isPaused)
        {
            if (IsPauseHeld && isPaused == false)
                Time.timeScale = 0f;
        }
    }
}
