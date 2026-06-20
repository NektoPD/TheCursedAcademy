using System;
using System.Collections;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class RagemodeAbility : AbilityBase
    {
        public event Action<float> RageModeStarted;
        public event Action RageModeEnded;

        protected override void Execute()
        {
            IsActive = true;
            StartCoroutine(RageModeRoutine());
        }

        private IEnumerator RageModeRoutine()
        {
            float damageMult = Config.Damage;
            RageModeStarted?.Invoke(damageMult);

            yield return new WaitForSeconds(Config.Duration);

            RageModeEnded?.Invoke();
            IsActive = false;
        }
    }
}
