using System;
using UnityEngine;

namespace Items.ItemVariations.MagicRuller
{
    public class RulerProjectile : ItemProjectile
    {
        [SerializeField, Min(0.05f)] private float _lifetime = 3f;

        private Vector2 _direction;
        private float _speed;
        private float _remainingLifetime;

        public event Action<RulerProjectile> Expired;

        public void Launch(Vector2 direction, float speed)
        {
            _direction = direction.normalized;
            _speed = speed;
            _remainingLifetime = Mathf.Max(0.05f, _lifetime);
        }

        private void OnDisable()
        {
            _remainingLifetime = 0f;
        }

        private void Update()
        {
            if (_remainingLifetime <= 0f)
                return;

            float step = Mathf.Min(Time.deltaTime, _remainingLifetime);
            Transform.Translate(_direction * _speed * step, Space.World);
            _remainingLifetime -= step;

            if (_remainingLifetime <= 0f)
                Expired?.Invoke(this);
        }
    }
}