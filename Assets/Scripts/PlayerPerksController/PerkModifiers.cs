using System.Collections.Generic;
using UnityEngine;

namespace PlayerPerksController
{
    [CreateAssetMenu(fileName = "Perk Modifiers", menuName = "PerkModifiers/Create new perk modifier")]
    public class PerkModifiers : ScriptableObject
    {
        [SerializeField] private float _powerModifier;
        [SerializeField] private float _armorModifier;
        [SerializeField] private float _maxHpModifier;
        [SerializeField] private float _hpRegenerationModifier;
        [SerializeField] private float _attackCooldownModifier;
        [SerializeField] private float _speedModifier;

        public Dictionary<PerkType, float> GetModifiers()
        {
            return new Dictionary<PerkType, float>
            {
                { PerkType.Power, _powerModifier },
                { PerkType.Armor, _armorModifier },
                { PerkType.MaxHp, _maxHpModifier },
                { PerkType.HpRegeneration, _hpRegenerationModifier },
                { PerkType.AttackCooldown, _attackCooldownModifier },
                { PerkType.Speed, _speedModifier }
            };
        }
    }
}