using Items.Enums;
using Items.ItemData;
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
        [SerializeField] private ItemDataConfig _config;
        [SerializeField] private List<StatVisualData> _stats;
        [field: SerializeField] public ItemVariations Variation {  get; private set; }

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);

        public Sprite Sprite => _config.ItemIcon;

        public ItemDataConfig Config => _config;

        public IReadOnlyList<StatVisualData> Stats => _stats;
    }
}