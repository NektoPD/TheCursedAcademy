using Agava.WebUtility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioLogic
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSingleton : MonoBehaviour
    {
        private static AudioSingleton s_instance;

        [SerializeField] private AudioClip _menu;
        [SerializeField] private AudioClip _level;
        [SerializeField] private AudioClip _tutorial;
        [SerializeField] private int _idMenuScene;
        [SerializeField] private int _idTutorialScene;
        [SerializeField] private int _idGameScene;

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
            int currentSceneIndex = second.buildIndex;
            AudioClip newClip = null;

            if (currentSceneIndex == _idMenuScene)
            {
                newClip = _menu;
            }
            else if (currentSceneIndex == _idTutorialScene)
            {
                newClip = _tutorial;
            }
            else if (currentSceneIndex == _idGameScene)
            {
                newClip = _level;
            }
            else
            {
                newClip = _level;
            }

            if (_source.clip == newClip) return;
            _source.clip = newClip;
            _source.Play();
        }

        private void OnInBackgroundChange(bool inBackground)
        {
            AudioListener.pause = inBackground;
            AudioListener.volume = inBackground ? 0f : 1f;
        }
    }
}