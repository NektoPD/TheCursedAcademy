using UnityEngine;
using UnityEngine.UI;

namespace AudioLogic
{
    [RequireComponent(typeof(AudioSaver), typeof(Audio))]
    public class AudioApplicator : MonoBehaviour
    {
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

            AudioSetting linearSettings = new AudioSetting(
                Audio.GetLinearVolume(settings.MasterVolume),
                Audio.GetLinearVolume(settings.AmbientVolume),
                Audio.GetLinearVolume(settings.SfxVolume));

            ApplyAudioSetting(linearSettings);
            ApplySlidersSettings(linearSettings);
        }

        private void ApplySlidersSettings(AudioSetting setting)
        {
            _masterVolume.SetValueWithoutNotify(setting.MasterVolume);
            _ambientVolume.SetValueWithoutNotify(setting.AmbientVolume);
            _sfxVolume.SetValueWithoutNotify(setting.SfxVolume);
        }

        private void ApplyAudioSetting(AudioSetting setting)
        {
            _audio.ChangeMasterVolume(setting.MasterVolume);
            _audio.ChangeAmbientVolume(setting.AmbientVolume);
            _audio.ChangeSfxVolume(setting.SfxVolume);
        }
    }
}