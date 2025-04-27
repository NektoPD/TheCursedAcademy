namespace AudioLogic
{
    public class AudioSetting
    {
        public AudioSetting(float masterVolume, float ambientVolume, float sfxVolume)
        {
            MasterVolume = masterVolume;
            AmbientVolume = ambientVolume;
            SfxVolume = sfxVolume;
        }

        public float MasterVolume { get; private set; }

        public float AmbientVolume { get; private set; }

        public float SfxVolume { get; private set; }
    }
}