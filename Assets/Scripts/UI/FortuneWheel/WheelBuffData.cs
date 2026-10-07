using UnityEngine;
using Utils;

namespace UI.FortuneWheel
{
    [CreateAssetMenu(fileName = "WheelBuffData", menuName = "FortuneWheel/WheelBuffData")]
    public class WheelBuffData : ScriptableObject
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;

        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public PerkType Type { get; private set; }
        [field: SerializeField] public float Multiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float DurationSeconds { get; private set; } = 10f;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);
    }
}
