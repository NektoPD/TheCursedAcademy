using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.Toys
{
    public class ToysProjectile : ItemProjectile
    {
        [SerializeField] private float _rotationSpeed = 360f;

        public Transform Transform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Transform = transform;
        }

        public override void Initialize(float damage, Item owner)
        {
            Initialize(damage, owner);
        }

        private void Update()
        {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
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