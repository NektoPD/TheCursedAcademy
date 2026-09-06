using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic;
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
        protected IEnumerable<StatVariations> StatVariations;
        protected CharacterSoundController CharacterSoundController;

        private bool _canAttack = true;
        private IEnumerator _attackCoroutine;

        protected int Level = 1;
        protected float RuntimeCooldown;
        protected float RuntimeDamage;
        private Func<bool> _isBerserkActive;
        
        protected StatModifiers Mods = new();
        public IReadOnlyList<Stat> UiStats => ItemStats.Stats;

        [field: SerializeField] public ItemDataConfig Data { get; private set; }
        [field: SerializeField] public ItemVisualData VisualData { get; private set; }

        public int CurrentLevel => Level;
        public bool IsBerserkActive => _isBerserkActive?.Invoke() == true;
        public event Action<Enums.ItemVariations, float> DamageDealt;
        public event Action MaxLevelReached;

        public void Initialize(CharacterMovementHandler movementHandler,
            CharacterSoundController characterSoundController, Func<bool> isBerserkActive)
        {
            MovementHandler = movementHandler;
            ItemStats = new ItemStats(VisualData.Stats);
            ItemStats.Item = VisualData.Name;
            StatVariations = VisualData.Stats.Select(stat => stat.Variation);
            CharacterSoundController = characterSoundController;
            _isBerserkActive = isBerserkActive;
            RuntimeCooldown = Data.Cooldown;
            RuntimeDamage = Data.Damage;
            
            UpdateStatsValues();
        }
        
        public IReadOnlyList<Stat> GetUiStats() => VisualData.Stats;
        
        public void Attack()
        {
            if (!_canAttack) return;
            PerformAttack();
            StartCoroutine(AttackCooldown());
        }

        public void RaiseDamageDealt(float damage)
        {
            DamageDealt?.Invoke(Data.ItemVariation, damage);
        }

        public virtual void LevelUp()
        {
        }

        public bool IsMaxLevelReached()
        {
            return CurrentLevel >= Data.MaxLevel;
        }
        
        protected void UpdateUiStat(StatVariations v, float current, float next)
        {
            ItemStats.SetStatCurrentValue(v, current);
            ItemStats.SetStatNextValue(v, next);
        }

        protected virtual float GetBaseStat(StatVariations v)
        {
            return v switch
            {
                Enums.StatVariations.Damage => Data.Damage,
                Enums.StatVariations.AttackSpeed => Data.Cooldown,
                _ => 0f
            };
        }
        
        protected void RaiseMaxLevelReached()
        {
            MaxLevelReached?.Invoke();
        }

        protected float GetCurrentStat(StatVariations v) => GetBaseStat(v) * Mods.GetMult(v);
        protected float GetNextStat(StatVariations v, float nextMult) => GetBaseStat(v) * nextMult;
        
        protected abstract void UpdateStatsValues();

        protected abstract void PerformAttack();

        private IEnumerator AttackCooldown()
        {
            _canAttack = false;
            yield return new WaitForSeconds(RuntimeCooldown);
            _canAttack = true;
        }
    }
}