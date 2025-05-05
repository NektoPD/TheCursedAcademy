using System.Collections;
using System.Collections.Generic;
using EnemyLogic;
using HealthSystem;
using Items.Interfaces;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations.SchoolBell
{
    public class SchoolBellProjectile : ItemProjectile
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _ringAnimationTrigger = "Ring";

        private float _freezeDuration;
        private float _freezeRadius;
        private LayerMask _enemyLayerMask;
        private readonly Dictionary<Enemy, Coroutine> _frozenEnemies = new Dictionary<Enemy, Coroutine>();

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(_ringAnimationTrigger);

                AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float animationLength = clipInfo[0].clip.length;

                    StartCoroutine(DeactivateAfterAnimation(animationLength));
                }
            }

            FreezeSurroundingEnemies();
        }

        private IEnumerator DeactivateAfterAnimation(float animationLength)
        {
            yield return new WaitForSeconds(animationLength);

            if (gameObject.activeSelf)
            {
                if (Owner && Owner.TryGetComponent(out ItemProjectilePool pool))
                {
                    pool.ReturnToPool(this);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void SetFreezeDuration(float duration)
        {
            _freezeDuration = duration;
        }

        public void SetFreezeRadius(float radius)
        {
            _freezeRadius = radius;
        }

        public void SetEnemyLayerMask(LayerMask layerMask)
        {
            _enemyLayerMask = layerMask;
        }

        private void FreezeSurroundingEnemies()
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                Transform.position,
                _freezeRadius,
                _enemyLayerMask);

            foreach (Collider2D enemyCollider in enemiesInRange)
            {
                if (!enemyCollider.TryGetComponent(out Enemy enemy))
                    return;

                if (enemy.TryGetComponent(out IFreezable freezable) && !_frozenEnemies.ContainsKey(enemy))
                {
                    Coroutine freezeCoroutine = StartCoroutine(FreezeEnemy(freezable, enemy));
                    _frozenEnemies.Add(enemy, freezeCoroutine);
                }
            }
        }

        private IEnumerator FreezeEnemy(IFreezable freezable, Enemy enemy)
        {
            freezable.Freeze();

            yield return new WaitForSeconds(_freezeDuration);

            if (enemy && freezable != null)
            {
                freezable.Unfreeze();
            }

            _frozenEnemies.Remove(enemy);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage * 0.1f);
            }
        }

        private void OnDisable()
        {
            foreach (var pair in _frozenEnemies)
            {
                if (pair.Key != null && pair.Key.TryGetComponent(out IFreezable freezable))
                {
                    freezable.Unfreeze();
                }

                if (pair.Value != null)
                {
                    StopCoroutine(pair.Value);
                }
            }

            _frozenEnemies.Clear();
        }
    }
}