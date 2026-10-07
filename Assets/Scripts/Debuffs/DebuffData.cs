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
        [SerializeField] private List<Sprite> _iconVariants = new();
        [SerializeField, Range(0f, 0.5f)] private float _maxDeviation = 0.1f;
        [SerializeField] private List<DebuffModifier> _modifiers = new();

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);
        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);
        public IReadOnlyList<DebuffModifier> Modifiers => _modifiers;
        public IReadOnlyList<Sprite> IconVariants => _iconVariants;
        public int VariantCount => _iconVariants.Count > 0 ? _iconVariants.Count : 1;

        public Sprite GetIcon(int variantIndex)
        {
            if (_iconVariants.Count == 0)
                return Icon;

            int index = Mathf.Clamp(variantIndex, 0, _iconVariants.Count - 1);

            return _iconVariants[index] != null ? _iconVariants[index] : Icon;
        }

        public float GetDeviation(int variantIndex)
        {
            if (VariantCount <= 1)
                return 0f;

            int index = Mathf.Clamp(variantIndex, 0, VariantCount - 1);
            float t = (float)index / (VariantCount - 1);

            return Mathf.Lerp(-_maxDeviation, _maxDeviation, t);
        }

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

    public class DebuffRoll
    {
        public DebuffData Data { get; }
        public int VariantIndex { get; }
        public float Deviation { get; }

        public DebuffRoll(DebuffData data, int variantIndex)
        {
            Data = data;
            VariantIndex = variantIndex;
            Deviation = data != null ? data.GetDeviation(variantIndex) : 0f;
        }

        public Sprite Icon => Data != null ? Data.GetIcon(VariantIndex) : null;
        public string Name => Data != null ? Data.Name : string.Empty;
        public string Description => Data != null ? Data.Description : string.Empty;

        public IEnumerable<DebuffModifier> GetModifiers(float negativeEffectMultiplier = 1f)
        {
            if (Data == null)
                yield break;

            foreach (DebuffModifier modifier in Data.Modifiers)
            {
                float multiplier = ApplyDeviation(modifier.Multiplier);

                if (IsNegative(modifier.Type, multiplier))
                    multiplier = 1f + (multiplier - 1f) * Mathf.Max(1f, negativeEffectMultiplier);

                yield return new DebuffModifier(modifier.Type, Mathf.Max(0.01f, multiplier));
            }
        }

        private static bool IsNegative(PerkType type, float multiplier)
        {
            return type == PerkType.AttackCooldown
                ? multiplier > 1f
                : multiplier < 1f;
        }

        private float ApplyDeviation(float multiplier)
        {
            return 1f + (multiplier - 1f) * (1f + Deviation);
        }
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
