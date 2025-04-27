using UnityEngine;
using UnityEngine.UI;

namespace AudioLogic
{
    [RequireComponent(typeof(AudioSaver), typeof(Audio))]
    public class AudioApplicator : MonoBehaviour
    {
        private readonly int _logarithmBase = 10;
        private readonly int _linearToAttenuationLevel = 20;

        [SerializeField] private Slider _masterVolume;
        [SerializeField] private Slider _ambientVolume;
        [SerializeField] private Slider _sfxVolume;

        private AudioSaver _audioSaver;
        private Audio _audio;

        private void Awake()
        {
            _audio = GetComponent<Audio>();
            _audioSaver = GetComponent<AudioSaver>();
        }

        private void Start()
        {
            AudioSetting settings = _audioSaver.Load();

            if (settings == null)
                return;

            ApplyAudioSetting(settings);
            ApplySlidersSettings(settings);
        }

        private void ApplySlidersSettings(AudioSetting setting)
        {
            _masterVolume.value = Mathf.Pow(_logarithmBase, setting.MasterVolume / _linearToAttenuationLevel);
            _ambientVolume.value = Mathf.Pow(_logarithmBase, setting.AmbientVolume / _linearToAttenuationLevel);
            _sfxVolume.value = Mathf.Pow(_logarithmBase, setting.SfxVolume / _linearToAttenuationLevel);
        }

        private void ApplyAudioSetting(AudioSetting setting)
        {
            _audio.ChangeMasterVolume(setting.MasterVolume);
            _audio.ChangeAmbientVolume(setting.AmbientVolume);
            _audio.ChangeSfxVolume(setting.SfxVolume);
        }
    }
}