using CharacterLogic.Data;
using UnityEngine;
using Utils;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterVisualData", menuName = "VisualData/CharacterVisualData ", order = 1)]
    public class CharacterVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private string _nameRu;
        [SerializeField] private string _nameEn;
        [SerializeField] private string _nameTr;
        [SerializeField] private string _descriptionRu;
        [SerializeField] private string _descriptionEn;
        [SerializeField] private string _descriptionTr;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private CharacterData _data;

        public string Name => Translator.Translate(_nameRu, _nameEn, _nameTr);

        public string Description => Translator.Translate(_descriptionRu, _descriptionEn, _descriptionTr);

        public Sprite Sprite => _sprite;

        public CharacterData Data => _data;
    }
}
