using System;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.Cross
{
    public class CrossProjectile : ItemProjectile
    {
        private enum Phase
        {
            Outbound,
            Returning
        }

        private Vector2 _direction;
        private float _speed;
        private float _maxTravelDistance;
        private Transform _ownerTransform;
        private Vector2 _startPosition;
        private Phase _phase = Phase.Outbound;
        private float _traveledDistance;

        public event Action<CrossProjectile> Finished;

        public void Launch(Vector2 direction, float speed, float maxTravelDistance, Transform ownerTransform)
        {
            _direction = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;
            _speed = speed;
            _maxTravelDistance = maxTravelDistance;
            _ownerTransform = ownerTransform;
            _startPosition = Transform.position;
            _phase = Phase.Outbound;
            _traveledDistance = 0f;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnEnable()
        {
            _phase = Phase.Outbound;
            _traveledDistance = 0f;
            SpriteRenderer.flipX = false;
            ClearHitEnemies();
        }

        private void Update()
        {
            if (_phase == Phase.Outbound)
            {
                MoveOutbound();
                return;
            }

            MoveReturning();
        }

        private void MoveOutbound()
        {
            float step = _speed * Time.deltaTime;
            Transform.position += (Vector3)(_direction * step);
            _traveledDistance += step;

            if (_traveledDistance < _maxTravelDistance)
                return;

            BeginReturn();
        }

        private void BeginReturn()
        {
            _phase = Phase.Returning;
            ClearHitEnemies();

            Vector2 returnTarget = _ownerTransform != null
                ? (Vector2)_ownerTransform.position
                : _startPosition;

            _direction = (returnTarget - (Vector2)Transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            SpriteRenderer.flipX = true;
        }

        private void MoveReturning()
        {
            Vector2 returnTarget = _ownerTransform != null
                ? (Vector2)_ownerTransform.position
                : _startPosition;

            if (Vector2.Distance(Transform.position, returnTarget) <= 0.35f)
            {
                Finished?.Invoke(this);
                return;
            }

            _direction = (returnTarget - (Vector2)Transform.position).normalized;
            Transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character) &&
                HitEnemies.Add(damageable))
            {
                damageable.TakeDamage(Damage, IsBerserkDamage);
                Owner?.RaiseDamageDealt(Damage);
            }
        }
    }
}
