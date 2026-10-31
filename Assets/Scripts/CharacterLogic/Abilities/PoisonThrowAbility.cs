using System.Collections;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonThrowAbility : AbilityBase
    {
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
                ActivationEffect.gameObject.SetActive(false);
                _halfwayReached = false;

                ActivationEffect.HalfwayReached += OnHalfwayReached;
                ActivationEffect.gameObject.SetActive(true);

                yield return new WaitUntil(() => _halfwayReached);

                ActivationEffect.HalfwayReached -= OnHalfwayReached;
            }

            IsActive = false;
            SpawnAreaAttack();
        }

        private void OnHalfwayReached()
        {
            _halfwayReached = true;
        }

        private void SpawnAreaAttack()
        {
            AbilityProjectile projectile = Instantiate(Config.ProjectilePrefab, OwnerTransform.position, Quaternion.identity);
            projectile.transform.SetParent(null);
            projectile.Launch(Vector2.zero, 0f, Config.Damage, Config.Duration);
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (ActivationEffect != null)
            {
                ActivationEffect.HalfwayReached -= OnHalfwayReached;
                ActivationEffect.gameObject.SetActive(false);
            }

            IsActive = false;
        }
    }
}
