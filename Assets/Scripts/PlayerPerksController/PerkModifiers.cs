using System.Collections.Generic;
using UnityEngine;

namespace PlayerPerksController
{
    [CreateAssetMenu(fileName = "Perk Modifiers", menuName = "PerkModifiers/Create new perk modifier")]
    public class PerkModifiers : ScriptableObject
    {
        [SerializeField] private float _powerModifier = 0.05f;
        [SerializeField] private float _armorModifier = 0.05f;
        [SerializeField] private float _maxHpModifier = 0.05f;
        [SerializeField] private float _hpRegenerationModifier = 0.05f;
        [SerializeField] private float _attackCooldownModifier = -0.05f;
        [SerializeField] private float _speedModifier = 0.05f;
        [SerializeField] private float _greedModifier = 0.05f;
        [SerializeField] private float _growthModifier = 0.05f;
        [SerializeField] private float _magnetModifier = 0.1f;
        [SerializeField] private float _areaModifier = 0.05f;
        [SerializeField] private float _durationModifier = 0.05f;

        public Dictionary<PerkType, float> GetModifiers()
        {
            return new Dictionary<PerkType, float>
            {
                { PerkType.Power, _powerModifier }, { PerkType.Armor, _armorModifier },
                { PerkType.MaxHp, _maxHpModifier }, { PerkType.HpRegeneration, _hpRegenerationModifier },
                { PerkType.AttackCooldown, _attackCooldownModifier }, { PerkType.Speed, _speedModifier },
                { PerkType.Greed, _greedModifier }, { PerkType.Growth, _growthModifier },
                { PerkType.Magnet, _magnetModifier }, { PerkType.Area, _areaModifier },
                { PerkType.Duration, _durationModifier }
            };
        }
    }
}