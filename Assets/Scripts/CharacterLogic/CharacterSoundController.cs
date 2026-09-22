using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterLogic
{
    [Serializable]
    public class SoundData
    {
        public SoundType SoundType;
        public AudioSource AudioSource;
    }

    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField] private List<SoundData> _soundDatas;
        [SerializeField] private AudioSource _abilityAudioSource;

        public void EnableSoundByType(SoundType type)
        {
            _soundDatas?.FirstOrDefault(data => data != null && data.SoundType == type)?.AudioSource?.Play();
        }

        public void PlayAbilitySound(AudioClip clip, SoundType fallbackType)
        {
            if (clip == null)
            {
                EnableSoundByType(fallbackType);
                return;
            }

            AudioSource source = _abilityAudioSource != null
                ? _abilityAudioSource
                : _soundDatas?.FirstOrDefault(data => data != null && data.SoundType == fallbackType)?.AudioSource;

            if (source != null)
                source.PlayOneShot(clip);
        }
    }

    public enum SoundType
    {
        Bell,
        Book,
        Cats,
        Hit,
        LevelUp,
        Shield,
        Slash,
        Slingshot,
        Zone,
        Heal,
        GameOver,
        Coin,
        MaxLevel,
        FullInventory,
        XpBooster,
        XpPoint,
        Fireblast,
        Ragemode,
        PoisonThrow,
        CherryBombExplosion
    }
}