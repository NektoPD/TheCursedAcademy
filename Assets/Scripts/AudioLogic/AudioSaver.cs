using UnityEngine;

namespace AudioLogic
{
    public class AudioSaver : MonoBehaviour
    {
        private const string MasterKey = "Master_volume";
        private const string AmbientKey = "Ambient_volume";
        private const string SfxKey = "Sfx_volume";

        private readonly float _defaultValue = -1000f;

        public AudioSetting Load()
        {
            float master = PlayerPrefs.GetFloat(MasterKey, _defaultValue);
            float ambient = PlayerPrefs.GetFloat(AmbientKey, _defaultValue);
            float sfx = PlayerPrefs.GetFloat(SfxKey, _defaultValue);

            if (master == _defaultValue || ambient == _defaultValue || sfx == _defaultValue)
                return null;

            return new (master, ambient, sfx);
        }

        public void Save(float master, float ambient, float sfx)
        {
            PlayerPrefs.SetFloat(MasterKey, master);
            PlayerPrefs.SetFloat(AmbientKey, ambient);
            PlayerPrefs.SetFloat(SfxKey, sfx);
            PlayerPrefs.Save();
        }
    }
}