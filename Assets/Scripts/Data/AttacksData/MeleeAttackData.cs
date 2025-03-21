using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Attacks/MeleeAttackData ", order = 1)]
    public class MeleeAttackData : AttackData
    {
        [SerializeField] private float _damage;

        public float Damage => _damage;
    }
}
