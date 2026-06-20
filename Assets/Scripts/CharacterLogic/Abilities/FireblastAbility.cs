using System.Collections;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class FireblastAbility : AbilityBase
    {
        protected override void Execute()
        {
            IsActive = true;
            StartCoroutine(SpawnFireballs());
        }

        private IEnumerator SpawnFireballs()
        {
            int count = Config.ProjectileCount;
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                GameObject projectile = Instantiate(Config.ProjectilePrefab, OwnerTransform.position, Quaternion.identity);
                projectile.transform.SetParent(null);

                float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);

                FireblastProjectile fb = projectile.GetComponent<FireblastProjectile>();
                if (fb != null)
                    fb.Launch(direction, Config.ProjectileSpeed, Config.Damage, Config.Duration);
            }

            yield return new WaitForSeconds(Config.Duration);
            IsActive = false;
        }
    }
}
