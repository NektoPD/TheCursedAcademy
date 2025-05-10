using System.Collections;
using CharacterLogic.InputHandler;
using Data;
using Items.Interfaces;
using Items.ItemData;
using UnityEngine;

namespace Items.BaseClass
{
    public abstract class Item : MonoBehaviour, IAttackable
    {
        protected CharacterMovementHandler MovementHandler;

        private bool _canAttack = true;
        private IEnumerator _attackCoroutine;

        protected int Level = 1;

        [field: SerializeField] public ItemDataConfig Data { get; private set; }
        [field: SerializeField] public ItemVisualData VisualData { get; private set; }

        public int CurrentLevel => Level;

        public void Initialize(CharacterMovementHandler movementHandler)
        {
            MovementHandler = movementHandler;
        }

        public void Attack()
        {
            if (!_canAttack) return;
            PerformAttack();
            StartCoroutine(AttackCooldown());
        }

        public abstract void LevelUp();

        protected abstract void PerformAttack();

        private IEnumerator AttackCooldown()
        {
            var coolDown = new WaitForSeconds(Data.Cooldown);
            _canAttack = false;
            yield return coolDown;
            _canAttack = true;
        }
    }
}