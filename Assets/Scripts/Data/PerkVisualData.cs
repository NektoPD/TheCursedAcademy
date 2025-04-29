using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PerkVisualData", menuName = "VisualData/PerkVisualData ", order = 1)]
    public class PerkVisualData : ScriptableObject, IVisualData
    {
        [SerializeField] private PerkType _type;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _defaultPrice;

        public PerkType Type => _type;

        public string Name => _name;

        public string Description => _description;

        public Sprite Sprite => _sprite;

        public int DefaultPrice => _defaultPrice;
    }
}