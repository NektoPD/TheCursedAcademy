using System;
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
        private readonly Dictionary<EnemyFreezer, Coroutine> _frozenEnemies = new Dictionary<EnemyFreezer, Coroutine>();

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

        public void FreezeSurroundingEnemies()
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
                Transform.position,
                _freezeRadius,
                _enemyLayerMask);

            foreach (Collider2D enemyCollider in enemiesInRange)
            {
                if (enemyCollider.TryGetComponent(out EnemyFreezer freezable) && !_frozenEnemies.ContainsKey(freezable))
                {
                    Coroutine freezeCoroutine = StartCoroutine(FreezeEnemy(freezable));
                    _frozenEnemies.Add(freezable, freezeCoroutine);
                }
            }
        }

        private IEnumerator FreezeEnemy(EnemyFreezer freezable)
        {
            freezable.Freeze();

            yield return new WaitForSeconds(_freezeDuration);

            if (freezable != null)
            {
                freezable.Unfreeze();
            }

            _frozenEnemies.Remove(freezable);
        }

        private void OnDisable()
        {
            foreach (var pair in _frozenEnemies)
            {
                if (pair.Key != null)
                {
                    pair.Key.Unfreeze();
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