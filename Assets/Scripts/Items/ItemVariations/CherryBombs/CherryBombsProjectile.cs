using System;
using System.Collections.Generic;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.CherryBombs
{
    public class CherryBombsProjectile : ItemProjectile
    {
        [SerializeField] private float _explosionRadius = 1.5f;
        [SerializeField] private LayerMask _enemyLayer;

        private Vector2 _direction;
        private float _speed;
        private float _fuseTimer;
        private float _fuseDuration;
        private bool _hasExploded;

        public event Action<CherryBombsProjectile> Finished;

        public void Launch(Vector2 direction, float speed, float fuseDuration)
        {
            _direction = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;
            _speed = speed;
            _fuseDuration = fuseDuration;
            _fuseTimer = 0f;
            _hasExploded = false;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnEnable()
        {
            _fuseTimer = 0f;
            _hasExploded = false;
            ClearHitEnemies();
        }

        private void Update()
        {
            if (_hasExploded)
                return;

            BouncingProjectileUtility.MoveWithViewportBounce(Transform, ref _direction, _speed);

            _fuseTimer += Time.deltaTime;
            if (_fuseTimer >= _fuseDuration)
                Explode();
        }

        private void Explode()
        {
            if (_hasExploded)
                return;

            _hasExploded = true;

            Collider2D[] hits = Physics2D.OverlapCircleAll(Transform.position, _explosionRadius, _enemyLayer);
            var damaged = new HashSet<IDamageable>();

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out IDamageable damageable) ||
                    hit.TryGetComponent(out Character character) ||
                    !damaged.Add(damageable))
                    continue;

                damageable.TakeDamage(Damage);
                Owner?.RaiseDamageDealt(Damage);
            }

            Finished?.Invoke(this);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasExploded)
                return;

            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                HitEnemies.Add(damageable))
            {
                damageable.TakeDamage(Damage);
                Owner?.RaiseDamageDealt(Damage);
            }
        }
    }
}
