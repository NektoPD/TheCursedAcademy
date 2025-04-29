using CharacterLogic.Data;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterVisualData", menuName = "VisualData/CharacterVisualData ", order = 1)]
    public class CharacterVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private CharacterData _data;

        public string Name => _name;

        public string Description => _description;

        public Sprite Sprite => _sprite;

        public CharacterData Data => _data;
    }
}
