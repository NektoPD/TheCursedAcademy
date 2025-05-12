using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic.InputHandler;
using Data;
using Items.Enums;
using Items.Interfaces;
using Items.ItemData;
using Items.Stats;
using UnityEngine;

namespace Items.BaseClass
{
    public abstract class Item : MonoBehaviour, IAttackable
    {
        protected CharacterMovementHandler MovementHandler;
        protected ItemStats ItemStats;
        protected IEnumerable<StatVariations> _statVariations;

        private bool _canAttack = true;
        private IEnumerator _attackCoroutine;

        protected int Level = 1;

        [field: SerializeField] public ItemDataConfig Data { get; private set; }
        [field: SerializeField] public ItemVisualData VisualData { get; private set; }

        public int CurrentLevel => Level;

        public void Initialize(CharacterMovementHandler movementHandler)
        {
            MovementHandler = movementHandler;
            ItemStats = new ItemStats(VisualData);
            _statVariations = VisualData.Stats.Select(stat => stat.Variation);
        }

        public void Attack()
        {
            if (!_canAttack) return;
            PerformAttack();
            StartCoroutine(AttackCooldown());
        }

        public virtual void LevelUp() 
        {
            if(Level <= 3)
                ItemStats.UpgradeStats(_statVariations);
        }

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