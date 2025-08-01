using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.Toys
{
    public class ToysProjectile : ItemProjectile
    {
        [SerializeField] private float _rotationSpeed = 360f;

        public override void Initialize(float damage, Item owner)
        {
            base.Initialize(damage, owner);
        }

        private void Update()
        {
            Transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageable?.TakeDamage(Damage);
            }
        }
    }
}