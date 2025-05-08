using UnityEngine;
using Utils;

namespace Data
{
    [CreateAssetMenu(fileName = "PerkVisualData", menuName = "VisualData/PerkVisualData ", order = 1)]
    public class PerkVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;

        [field: SerializeField] public PerkType Type { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public int DefaultPrice { get; private set; }

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);
    }
}