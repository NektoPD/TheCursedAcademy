using UnityEngine;

namespace Data.AttacksData
{
    public abstract class AttackData : ScriptableObject
    {
        [field: SerializeField] public string NameInAnimator {  get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float AttackRange { get; private set; }
    }
}
