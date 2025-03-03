using System.Collections;
using Items.Interfaces;
using UnityEngine;
using Items.ItemData;

namespace Items
{
    public abstract class Item : MonoBehaviour, IAttackable
    {
        [SerializeField] private ItemDataConfig _data;

        private bool _canAttack = true;

        public void Attack()
        {
            if (!_canAttack) return;
            PerformAttack();
            StartCoroutine(AttackCooldown());
        }

        protected abstract void PerformAttack();

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;
            yield return new WaitForSeconds(_data.Cooldown);
            _canAttack = true;
        }
    }
}