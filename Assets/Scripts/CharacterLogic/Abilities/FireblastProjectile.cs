using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class FireblastProjectile : AbilityProjectile
    {
        private float _lifetime;

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            base.Launch(direction, speed, damage, duration);
            _lifetime = duration;
            Destroy(gameObject, _lifetime);
        }
    }
}
