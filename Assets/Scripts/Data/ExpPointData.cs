using ExpPoints;
using UnityEngine;

namespace Data.ExpPointsData
{
    [CreateAssetMenu(fileName = "ExpPointData", menuName = "ExpPoint/ExpPointData", order = 1)]
    public class ExpPointData : ScriptableObject, IData<ExpPoint>
    {
        [field: SerializeField] public int Point { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public ExpPoint Prefab { get; private set; }
    }
}
