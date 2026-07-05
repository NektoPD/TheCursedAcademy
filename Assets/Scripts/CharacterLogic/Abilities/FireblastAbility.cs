using System.Collections;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class FireblastAbility : AbilityBase
    {
        protected override void Execute()
        {
            IsActive = true;
            StartCoroutine(ActivationSequence());
        }

        private IEnumerator ActivationSequence()
        {
            if (Config.ActivationEffectPrefab != null)
            {
                GameObject effect = Instantiate(Config.ActivationEffectPrefab, OwnerTransform.position, Quaternion.identity);
                effect.transform.SetParent(OwnerTransform);
                effect.SetActive(true);

                yield return new WaitUntil(() => !effect.activeInHierarchy);

                Destroy(effect);
            }

            yield return StartCoroutine(SpawnFireballs());
        }

        private IEnumerator SpawnFireballs()
        {
            int count = Config.ProjectileCount;
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                AbilityProjectile projectile = Instantiate(Config.ProjectilePrefab, OwnerTransform.position, Quaternion.identity);
                projectile.transform.SetParent(null);

                float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);

                projectile.Launch(direction, Config.ProjectileSpeed, Config.Damage, Config.Duration);
            }

            yield return new WaitForSeconds(Config.Duration);
            IsActive = false;
        }
    }
}
