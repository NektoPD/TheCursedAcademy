using System.Collections;
using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonProjectile : AbilityProjectile
    {
        private const int PoisonTicks = 5;
        private const float MaxLifetime = 3f;

        [SerializeField] private float _spawnScaleDuration = 0.3f;
        [SerializeField] private float _despawnScaleDuration = 0.3f;
        [SerializeField] private float _speedMultiplier = 0.5f;
        [SerializeField] private SpriteRenderer _projectileSprite;
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private float _hitEffectScaleDuration = 0.2f;

        private float _poisonDuration;
        private float _speed;
        private Vector3 _targetScale;
        private Vector3 _hitEffectTargetScale;

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            _speed = speed * _speedMultiplier;
            _poisonDuration = duration;
            _targetScale = transform.localScale;

            if (_hitEffect != null)
            {
                _hitEffectTargetScale = _hitEffect.transform.localScale;
                _hitEffect.SetActive(false);
            }

            if (_projectileSprite != null)
                _projectileSprite.color = new Color(_projectileSprite.color.r, _projectileSprite.color.g, _projectileSprite.color.b, 1f);

            transform.localScale = Vector3.zero;
            transform.DOScale(_targetScale, _spawnScaleDuration).SetEase(Ease.OutBack);

            base.Launch(direction, _speed, damage, duration);

            float despawnDelay = MaxLifetime - _despawnScaleDuration;
            if (despawnDelay < 0f) despawnDelay = 0f;

            DOVirtual.DelayedCall(despawnDelay, StartDespawn);
        }

        public void SetFacing(bool facingLeft)
        {
            if (_projectileSprite != null)
                _projectileSprite.flipY = facingLeft;
        }

        private void StartDespawn()
        {
            if (this == null) return;
            transform.DOScale(Vector3.zero, _despawnScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }

        protected override void ApplyEffect(IDamageable target)
        {
            StartCoroutine(ApplyPoison(target));
            ShowHitEffect();
        }

        private void ShowHitEffect()
        {
            if (_hitEffect == null) return;

            GameObject effect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
            effect.SetActive(true);
            effect.transform.localScale = Vector3.zero;

            effect.transform.DOScale(_hitEffectTargetScale, _hitEffectScaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    effect.transform.DOScale(Vector3.zero, _hitEffectScaleDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => Destroy(effect));
                });
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

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
