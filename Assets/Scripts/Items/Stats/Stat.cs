using System;
using UnityEngine;
using Utils;

namespace Items.Stats
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private float _value;
        [SerializeField] private float _step;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public float CurrentValue => _value;

        public float NextValue => _value + _step;

        public void SetStep(float step) => _step = step;

        public void LevelUp() => _value += _step;
    }
}