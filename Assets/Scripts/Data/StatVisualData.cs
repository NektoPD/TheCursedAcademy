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

        private float _pastValue = 0;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public float CurrentValue => _value;

        public float PastValue => _pastValue;

        public void SetValue(float value)
        {
            _pastValue = _value;
            _value = value;
        }
    }
}