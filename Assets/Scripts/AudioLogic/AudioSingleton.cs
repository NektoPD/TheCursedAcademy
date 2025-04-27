using Agava.WebUtility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSingleton : MonoBehaviour
    {
        private const string Menu = nameof(Menu);

        private static AudioSingleton s_instance;

        [SerializeField] private AudioClip _menu;
        [SerializeField] private AudioClip _level;

        private AudioSource _source;

        private void Awake()
        {
            WebApplication.CallbackLogging = true;

            _source = GetComponent<AudioSource>();

            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
            WebApplication.InBackgroundChangeEvent += OnInBackgroundChange;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            WebApplication.InBackgroundChangeEvent -= OnInBackgroundChange;
        }

        private void OnSceneChanged(Scene first, Scene second)
        {
            if (second.name == Menu)
                _source.clip = _menu;
            else if (_source.clip != _level)
                _source.clip = _level;
            else
                return;

            _source.Play();
        }

        private void OnInBackgroundChange(bool inBackground)
        {
            AudioListener.pause = inBackground;
            AudioListener.volume = inBackground ? 0f : 1f;
        }
    }
}