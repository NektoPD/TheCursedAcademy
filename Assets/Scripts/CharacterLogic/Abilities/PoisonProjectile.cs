using System.Collections;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonProjectile : AbilityProjectile
    {
        private const int PoisonTicks = 5;
        private const float MaxLifetime = 3f;

        private float _poisonDuration;

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            base.Launch(direction, speed, damage, duration);
            _poisonDuration = duration;
            Destroy(gameObject, MaxLifetime);
        }

        protected override void ApplyEffect(IDamageable target)
        {
            StartCoroutine(ApplyPoison(target));
        }

        private IEnumerator ApplyPoison(IDamageable target)
        {
            float tickDamage = Damage / PoisonTicks;
            float tickInterval = _poisonDuration / PoisonTicks;

            for (int i = 0; i < PoisonTicks; i++)
            {
                if ((target as Object) == null) yield break;
                target.TakeDamage(tickDamage);
                yield return new WaitForSeconds(tickInterval);
            }
        }
    }
}
