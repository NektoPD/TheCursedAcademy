using System;
using System.Collections;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class RagemodeAbility : AbilityBase
    {
        public event Action<float, float, float> RageModeStarted;
        public event Action RageModeEnded;

        protected override void Execute()
        {
            IsActive = true;

            if (ActivationEffect != null)
                ActivationEffect.gameObject.SetActive(true);

            StartCoroutine(RageModeRoutine());
        }

        private IEnumerator RageModeRoutine()
        {
            float damageMult = Config.Damage;
            float speedMult = Config.SpeedMultiplier;
            float armorMult = Config.ArmorMultiplier;
            RageModeStarted?.Invoke(damageMult, speedMult, armorMult);

            yield return new WaitForSeconds(Config.Duration);

            if (ActivationEffect != null)
                ActivationEffect.gameObject.SetActive(false);

            RageModeEnded?.Invoke();
            IsActive = false;
        }
    }
}
