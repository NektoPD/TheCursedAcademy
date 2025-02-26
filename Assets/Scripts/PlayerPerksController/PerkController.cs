using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace PlayerPerksController
{
    public class PerkController
    {
        private const int MaxUpgradeCount = 4;

        private readonly PerkModifiers _perkModifiers = ScriptableObject.CreateInstance<PerkModifiers>();

        public void Initialize()
        {
            if (YandexGame.SDKEnabled)
            {
                PerkDataWrapper = YandexGame.savesData.PerkDataWrapper;
            }
            else
            {
                PerkDataWrapper = new PerkDataWrapper();
            }
        }

        public PerkDataWrapper PerkDataWrapper { get; private set; }

        public void UpgradePerk(PerkType perkType)
        {
            if (PerkDataWrapper.PerkLevels.ContainsKey(perkType) &&
                PerkDataWrapper.PerkLevels[perkType] < MaxUpgradeCount)
            {
                PerkDataWrapper.PerkLevels[perkType]++;
            }

            YandexGame.SaveProgress();
        }

        public Dictionary<PerkType, float> GetFinalPerkValues()
        {
            Dictionary<PerkType, float> finalValues = new();
            Dictionary<PerkType, float> modifiers = _perkModifiers.GetModifiers();

            foreach (var perk in PerkDataWrapper.PerkLevels)
            {
                if (modifiers.TryGetValue(perk.Key, out float modifier))
                {
                    finalValues[perk.Key] = modifier * perk.Value;
                }
            }

            return finalValues;
        }
    }

    [Serializable]
    public class PerkDataWrapper
    {
        public Dictionary<PerkType, int> PerkLevels = new()
        {
            { PerkType.Power, 0 },
            { PerkType.Armor, 0 },
            { PerkType.MaxHp, 0 },
            { PerkType.HpRegeneration, 0 },
            { PerkType.AttackCooldown, 0 },
            { PerkType.Speed, 0 }
        };
    }
}