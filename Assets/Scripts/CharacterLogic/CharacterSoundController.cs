using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

        public void EnableSoundByType(SoundType type)
        {
            _soundDatas.FirstOrDefault(data => data.SoundType == type)?.AudioSource?.Play();
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
        GameOver
    }
}