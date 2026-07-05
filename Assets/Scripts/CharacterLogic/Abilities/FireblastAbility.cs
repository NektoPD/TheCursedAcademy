using System.Collections;
using UnityEngine;
using Utils;

namespace CharacterLogic.Abilities
{
    public class FireblastAbility : AbilityBase
    {
        private bool _activationFinished;
        private bool _halfwayReached;

        protected override void Execute()
        {
            IsActive = true;
            StartCoroutine(ActivationSequence());
        }

        private IEnumerator ActivationSequence()
        {
            if (ActivationEffect != null)
            {
                _activationFinished = false;
                _halfwayReached = false;

                ActivationEffect.Finished += OnActivationFinished;
                ActivationEffect.HalfwayReached += OnHalfwayReached;
                ActivationEffect.gameObject.SetActive(true);

                yield return new WaitUntil(() => _halfwayReached);

                SpawnFireballs();

                yield return new WaitUntil(() => _activationFinished);

                ActivationEffect.Finished -= OnActivationFinished;
                ActivationEffect.HalfwayReached -= OnHalfwayReached;
            }
            else
            {
                SpawnFireballs();
            }

            yield return new WaitForSeconds(Config.Duration);
            IsActive = false;
        }

        private void OnHalfwayReached()
        {
            _halfwayReached = true;
        }

        private void OnActivationFinished()
        {
            _activationFinished = true;
        }

        private void SpawnFireballs()
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
        }
    }
}
