using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

namespace CharacterLogic
{
    public class CharacterAttacker : MonoBehaviour
    {
        private CharacterInventory _inventory;
        private float _attackRegenerationSpeed;
        private bool _isAttacking = false;
        private IEnumerator _attackCoroutine;

        public void Initialize(CharacterInventory characterInventory, float attackRegenerationSpeed)
        {
            _inventory = characterInventory;
            _attackRegenerationSpeed = attackRegenerationSpeed;
        }

        private void OnEnable()
        {
            if (!_isAttacking && _inventory != null)
            {
                StartCoroutine(AutoAttackCoroutine());
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _isAttacking = false;
        }

        public void EnableAttack()
        {
            if (_attackCoroutine != null)
                return;

            _attackCoroutine = AutoAttackCoroutine();
            StartCoroutine(_attackCoroutine);
        }

        public void DisableAttack()
        {
            if (_attackCoroutine == null)
                return;

            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        private IEnumerator AutoAttackCoroutine()
        {
            _isAttacking = true;

            WaitForSeconds interval = new WaitForSeconds(_attackRegenerationSpeed);

            while (enabled && _inventory != null)
            {
                AttackWithAllItems();

                yield return interval;
            }

            _isAttacking = false;
        }

        private void AttackWithAllItems()
        {
            if (_inventory?.Items == null || !_inventory.Items.Any())
                return;

            foreach (var item in _inventory.Items)
            {
                item.Attack();
            }
        }
    }
}