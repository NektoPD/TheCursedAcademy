using UnityEngine;
using Utils;

namespace Data
{
    [CreateAssetMenu(fileName = "PerkVisualData", menuName = "VisualData/PerkVisualData ", order = 1)]
    public class PerkVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private PerkType _type;
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _defaultPrice;

        public PerkType Type => _type;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);

        public Sprite Sprite => _sprite;

        public int DefaultPrice => _defaultPrice;
    }
}