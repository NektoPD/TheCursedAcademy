using Items.Enums;
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
        [SerializeField] private StatVariations _variation;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public float CurrentValue => _value;

        public float NextValue { get; private set; }

        public StatVariations Variation => _variation;

        public void SetStep(float step) => _step = step;

        public void LevelUp() => _value += _step;

        public void SetCurrentValue(float value)
        {
            _value = value;
        }

        public void SetNextValue(float value)
        {
            NextValue = value;
        }
    }
}