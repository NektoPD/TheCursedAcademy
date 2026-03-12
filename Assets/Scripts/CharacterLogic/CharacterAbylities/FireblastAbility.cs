using System;
using System.Collections;
using UnityEngine;

namespace CharacterLogic.CharacterAbylities
{
    public class FireblastAbility : MonoBehaviour
    {
        [field: SerializeField] public float ChargeSpeed { get; private set; }
        [field: SerializeField] public float CooldownSpeed { get; private set; }
        [field: SerializeField] public float Range { get; private set; }

        public bool IsAvailable { get; private set; }
        
        private IEnumerator StartCharging()
        {
            float timer = 0;

            while (timer <= ChargeSpeed)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            UseAbility();
            IsAvailable = false;
        }

        private void UseAbility()
        {
            
        }

        private IEnumerator StartCooldown()
        {
            WaitForSeconds interval = new WaitForSeconds(CooldownSpeed);
            yield return interval;
            IsAvailable = true;
        }
    }
}
