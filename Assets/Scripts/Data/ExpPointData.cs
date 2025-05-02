using ExpPoints;
using UnityEngine;

namespace Data.ExpPointsData
{
    [CreateAssetMenu(fileName = "ExpPointData", menuName = "ExpPoint/ExpPointData", order = 1)]
    public class ExpPointData : ScriptableObject, IData<ExpPoint>
    {
        [SerializeField] private int _point;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [SerializeField] private ExpPoint _prefab;

        public int Point => _point;

        public Sprite Sprite => _sprite;

        public RuntimeAnimatorController AnimatorController => _animatorController;

        public ExpPoint Prefab => _prefab;
    }
}
