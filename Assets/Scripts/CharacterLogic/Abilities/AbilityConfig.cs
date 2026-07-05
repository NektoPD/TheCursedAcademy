using UnityEngine;

namespace CharacterLogic.Abilities
{
    [CreateAssetMenu(fileName = "New AbilityConfig", menuName = "Character/Create new ability config")]
    public class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public AbilityType Type { get; private set; }
        [field: SerializeField] public int KillsToCharge { get; private set; } = 50;
        [field: SerializeField] public float Damage { get; private set; } = 10f;
        [field: SerializeField] public float Duration { get; private set; } = 5f;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 8f;
        [field: SerializeField] public int ProjectileCount { get; private set; } = 12;
        [field: SerializeField] public AbilityProjectile ProjectilePrefab { get; private set; }
        [field: SerializeField] public SoundType ActivationSound { get; private set; }
        [field: SerializeField] public GameObject ActivationEffectPrefab { get; private set; }
    }
}
