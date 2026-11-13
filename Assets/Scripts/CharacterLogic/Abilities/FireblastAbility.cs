using System.Collections;
using System.Collections.Generic;
using EnemyLogic;
using UnityEngine;

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
                ActivationEffect.gameObject.SetActive(false);
                _activationFinished = false;
                _halfwayReached = false;

                ActivationEffect.Finished += OnActivationFinished;
                ActivationEffect.HalfwayReached += OnHalfwayReached;
                ActivationEffect.gameObject.SetActive(true);

                yield return new WaitUntil(() => _halfwayReached);

                DamageActivationArea();
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

        private void DamageActivationArea()
        {
            SpriteRenderer spriteRenderer = ActivationEffect.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                return;

            Sprite sprite = spriteRenderer.sprite;
            Vector2 size = sprite.rect.size / sprite.pixelsPerUnit;
            Vector2 localCenter = (sprite.rect.size * 0.5f - sprite.pivot) / sprite.pixelsPerUnit + Config.ActivationAreaOffset;

            if (spriteRenderer.flipX)
                localCenter.x = -localCenter.x;

            if (spriteRenderer.flipY)
                localCenter.y = -localCenter.y;

            Transform effectTransform = spriteRenderer.transform;
            Vector3 scale = effectTransform.lossyScale;
            float radius = Mathf.Min(size.x, size.y) * 0.5f *
                Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)) *
                Mathf.Max(0f, Config.ActivationAreaRadiusMultiplier);

            if (radius <= 0f)
                return;

            Vector2 center = effectTransform.TransformPoint(localCenter);
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, LayerMask.GetMask("Enemy"));
            HashSet<EnemyDamageTaker> hitEnemies = new HashSet<EnemyDamageTaker>();

            foreach (Collider2D hit in hits)
            {
                EnemyDamageTaker target = hit.GetComponentInParent<EnemyDamageTaker>();
                if (target == null || target.IsDied || !target.gameObject.activeInHierarchy || !hitEnemies.Add(target))
                    continue;

                target.TakeDamage(Config.Damage);
            }
        }

        private void SpawnFireballs()
        {
            int count = Config.ProjectileCount;
            if (count <= 0)
                return;

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

        private void OnDisable()
        {
            StopAllCoroutines();

            if (ActivationEffect != null)
            {
                ActivationEffect.Finished -= OnActivationFinished;
                ActivationEffect.HalfwayReached -= OnHalfwayReached;
                ActivationEffect.gameObject.SetActive(false);
            }

            IsActive = false;
        }
    }
}
