using System;
using System.Collections.Generic;
using CharacterLogic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class ItemProjectile : MonoBehaviour
    {
        protected float Damage;
        protected Item Owner;
        protected readonly HashSet<IDamageable> HitEnemies = new HashSet<IDamageable>();

        public SpriteRenderer SpriteRenderer { get; private set; }
        public Transform Transform { get; private set; }

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Transform = transform;
        }

        public virtual void Initialize(float damage, Item owner)
        {
            Damage = damage;
            Owner = owner;
        }

        public virtual void ClearHitEnemies()
        {
            HitEnemies.Clear();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out Character character))
            {
                damageable?.TakeDamage(Damage);
            }
        }
    }
}