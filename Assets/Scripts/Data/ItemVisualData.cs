using Items.Enums;
using Items.Stats;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Data
{
    [CreateAssetMenu(fileName = "PerkVisualData", menuName = "VisualData/ItemVisualData ", order = 1)]
    public class ItemVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;

        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public List<Stat> Stats { get; private set; }
        [field: SerializeField] public ItemVariations Variation {  get; private set; }

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);
    }
}