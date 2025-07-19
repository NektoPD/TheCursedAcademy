using UnityEngine;
using HealthSystem;
using Items.BaseClass;
using System.Collections;
using System.Collections.Generic;
using CharacterLogic;

namespace Items.ItemVariations
{
    public class PenProjectile : ItemProjectile
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }
    }
}