using System.Collections;
using Items.Interfaces;
using Items.ItemData;
using UnityEngine;

namespace Items.BaseClass
{
    public abstract class Item : MonoBehaviour, IAttackable
    {
        [field: SerializeField] public ItemDataConfig Data { get; private set; }

        private bool _canAttack = true;
        private IEnumerator _attackCoroutine;

        public void Attack()
        {
            if (!_canAttack) return;
            PerformAttack();
            StartCoroutine(AttackCooldown());
        }
        
        protected abstract void PerformAttack();

        protected abstract void LevelUp();

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;
            yield return new WaitForSeconds(Data.Cooldown);
            _canAttack = true;
        }
    }
}