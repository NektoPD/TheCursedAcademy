using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

namespace PlayerPerksController
{
    public class PerkController
    {
        public int MaxUpgradeCount { get; private set; } = 4;
        private readonly PerkModifiers _perkModifiers = ScriptableObject.CreateInstance<PerkModifiers>();

        public void Initialize()
        {
            PerkDataWrapper = YandexGame.savesData.PerkDataWrapper;
        }

        public PerkDataWrapper PerkDataWrapper { get; private set; }

        public int GetPerkLevel(PerkType type)
        {
            if (PerkDataWrapper.PerkLevels.ContainsKey(type) == false) throw new NullReferenceException(nameof(type));
            return PerkDataWrapper.PerkLevels[type];
        }

        public bool TryUpgradePerk(PerkType perkType)
        {
            if (PerkDataWrapper.PerkLevels.ContainsKey(perkType) &&
                PerkDataWrapper.PerkLevels[perkType] < MaxUpgradeCount)
            {
                PerkDataWrapper.PerkLevels[perkType]++;
                YandexGame.SaveProgress();
                return true;
            }

            return false;
        }

        public Dictionary<PerkType, float> GetFinalPerkValues()
        {
            var result = new Dictionary<PerkType, float>();
            var steps = _perkModifiers.GetModifiers();
            
            foreach (var kv in PerkDataWrapper.PerkLevels)
            {
                float step = steps.FirstOrDefault(v => v.Key == kv.Key).Value;
                int level = kv.Value;
                
                float multiplier = 1f + step * level;
                
                result[kv.Key] = multiplier;
            }

            return result;
        }
    }

    [Serializable]
    public class PerkDataWrapper
    {
        public Dictionary<PerkType, int> PerkLevels = new()
        {
            { PerkType.Power, 0 }, { PerkType.Armor, 0 }, { PerkType.MaxHp, 0 }, { PerkType.HpRegeneration, 0 },
            { PerkType.AttackCooldown, 0 }, { PerkType.Speed, 0 }
        };
    }
}