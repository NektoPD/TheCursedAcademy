using UnityEngine;

namespace AudioLogic
{
    [RequireComponent(typeof(AudioSaver))]
    public class Audio : MonoBehaviour
    {
        private const string AmbientVolume = nameof(AmbientVolume);
        private const string SfxVolume = nameof(SfxVolume);
        private const string MasterVolume = nameof(MasterVolume);

        private const float LinearToAttenuationLevel = 20f;
        private const float MinimumAttenuation = -80f;
        private readonly float _defaultValue = 0f;

        [SerializeField] private UnityEngine.Audio.AudioMixer _audioMixer;

        private AudioSaver _audioSaver;

        private void Awake()
        {
            _audioSaver = GetComponent<AudioSaver>();
        }

        public void ChangeAmbientVolume(float volume)
        {
            _audioMixer.SetFloat(AmbientVolume, GetAttenuation(volume));
            Save();
        }

        public void ChangeSfxVolume(float volume)
        {
            _audioMixer.SetFloat(SfxVolume, GetAttenuation(volume));
            Save();
        }

        public void ChangeMasterVolume(float volume)
        {
            _audioMixer.SetFloat(MasterVolume, GetAttenuation(volume));
            Save();
        }

        private static float GetAttenuation(float volume)
        {
            if (volume <= 0f)
                return MinimumAttenuation;

            return Mathf.Max(MinimumAttenuation,
                Mathf.Log10(Mathf.Clamp01(volume)) * LinearToAttenuationLevel);
        }

        internal static float GetLinearVolume(float attenuation)
        {
            if (attenuation <= MinimumAttenuation)
                return 0f;

            return Mathf.Clamp01(Mathf.Pow(10f, attenuation / LinearToAttenuationLevel));
        }

        private void Save()
        {
            _audioMixer.GetFloat(MasterVolume, out float master);
            _audioMixer.GetFloat(SfxVolume, out float sfx);
            _audioMixer.GetFloat(AmbientVolume, out float ambient);

            master = GetNormalVolume(master);
            sfx = GetNormalVolume(sfx);
            ambient = GetNormalVolume(ambient);

            _audioSaver.Save(master, ambient, sfx);
        }

        private float GetNormalVolume(float volume)
        {
            if (float.IsNormal(volume) == false)
                volume = _defaultValue;

            return volume;
        }
    }
}