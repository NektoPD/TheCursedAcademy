using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Debuffs
{
    [CreateAssetMenu(fileName = "DebuffData", menuName = "Debuffs/DebuffData")]
    public class DebuffData : ScriptableObject
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;

        [field: SerializeField] public Sprite Icon { get; private set; }
        [SerializeField] private List<DebuffModifier> _modifiers = new();

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);
        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);
        public IReadOnlyList<DebuffModifier> Modifiers => _modifiers;

        public void SetLocalization(string ru, string en, string tr,
            string descRu, string descEn, string descTr)
        {
            _nameRu = ru;
            _nameEn = en;
            _nameTr = tr;
            _descriptionRu = descRu;
            _descriptionEn = descEn;
            _descriptionTr = descTr;
        }

        public void SetIcon(Sprite icon) => Icon = icon;

        public void SetModifiers(List<DebuffModifier> modifiers) => _modifiers = modifiers;
    }

    [Serializable]
    public struct DebuffModifier
    {
        [field: SerializeField] public PerkType Type { get; private set; }
        [field: SerializeField] public float Multiplier { get; private set; }

        public DebuffModifier(PerkType type, float multiplier)
        {
            Type = type;
            Multiplier = multiplier;
        }
    }
}
