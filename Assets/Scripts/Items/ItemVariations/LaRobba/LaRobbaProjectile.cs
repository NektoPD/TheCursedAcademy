using System;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.LaRobba
{
    public class LaRobbaProjectile : ItemProjectile
    {
        private Vector2 _direction;
        private float _speed;
        private float _lifetime;
        private float _timer;

        public event Action<LaRobbaProjectile> Finished;

        public void Launch(Vector2 direction, float speed, float lifetime)
        {
            _direction = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.right;
            _speed = speed;
            _lifetime = lifetime;
            _timer = 0f;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnEnable()
        {
            _timer = 0f;
            ClearHitEnemies();
        }

        private void Update()
        {
            BouncingProjectileUtility.MoveWithViewportBounce(Transform, ref _direction, _speed);

            _timer += Time.deltaTime;
            if (_timer >= _lifetime)
                Finished?.Invoke(this);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
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
