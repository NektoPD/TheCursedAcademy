using System;
using UnityEngine;
using Utils;

namespace Data
{
    [Serializable]
    public class StatVisualData
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

        public void LevelUp() => _value = _value + _step;
    }
}