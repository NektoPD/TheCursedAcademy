using System;
using System.Collections.Generic;
using System.Linq;
using CameraExtensions;
using CharacterLogic.Data;
using CharacterLogic.InputHandler;
using EnemyLogic;
using HealthSystem;
using InventorySystem;
using Items.BaseClass;
using Items.Enums;
using Items.ItemHolder;
using Items.ItemVariations;
using Items.ItemVariations.MultiSlingshot;
using StatistiscSystem;
using UI.Applicators;
using UnityEngine;

namespace CharacterLogic
{
    [RequireComponent(typeof(CharacterAnimationController))]
    [RequireComponent(typeof(CharacterMovementHandler))]
    [RequireComponent(typeof(CharacterCollisionHandler))]
    [RequireComponent(typeof(CharacterSpriteHolder))]
    [RequireComponent(typeof(CharacterView))]
    [RequireComponent(typeof(CharacterAttacker))]
    public class Character : MonoBehaviour, IStatisticsTransmitter, IDamageable
    {
        [SerializeField] private CharacterInventoryUI _inventoryUI;
        [SerializeField] private bool _cameraOnCharacter;

        private CharacterData _characterData;
        private CharacterAnimationController _animationController;
        private CharacterMovementHandler _movementHandler;
        private Health _health;
        private CharacterSpriteHolder _spriteHolder;
        private CharacterCollisionHandler _collisionHandler;
        private CharacterView _view;
        private CharacterAttacker _attacker;
        private CharacterInventory _inventory;
        private ItemsHolder _itemsHolder;
        private CharacterLevelController _characterLevelController;
        private CharacterSessionWallet _characterSessionWallet;
        private ItemApplicator _itemApplicator;
        private KilledEnemyCounter _killedEnemyCounter;

        private float _attackPower;
        private float _armor;
        private float _hp;
        private float _hpRegenerationSpeed;
        private float _attackCooldown;
        private float _moveSpeed;
        private Item _startItem;
        private bool _isInvincible = false;
        private Transform _transform;
        private bool _isDied;
        private DateTime _gameStart;

        public event Action<Statistics> StatisticCollected;
        public event Action LevelUp;

        public CharacterInventory Inventory => _inventory;

        public bool IsDied => _isDied;

        public void Construct(CharacterData characterData, Dictionary<PerkType, float> perkBonuses,
            ItemsHolder itemsHolder, ItemApplicator itemApplicator, KilledEnemyCounter killedEnemyCounter)
        {
            _itemsHolder = itemsHolder;

            _collisionHandler = GetComponent<CharacterCollisionHandler>();

            InitializeCharacterComponents();
            InitializeCharacterData(characterData, perkBonuses);
            ActivateCharacter();

            _animationController.SetAnimatorOverride(characterData.AnimatorController);

            _movementHandler.MovingLeft += OnMovingLeft;
            _movementHandler.MovingRight += OnMovingRight;
            _health.Changed += UpdateHealthView;
            _health.LowHealth += _spriteHolder.StartPulsing;
            _health.Died += _spriteHolder.StopPulsing;
            _health.Died += OnPlayerDied;
            _health.HealthRegainedToNormal += _spriteHolder.StopPulsing;

            _collisionHandler.GotExpPoint += OnExperienceGained;
            _collisionHandler.GotHeal += TakeHeal;

            UpdateExperienceView(_characterLevelController.CurrentExp);

            _itemApplicator = itemApplicator;
            _killedEnemyCounter = killedEnemyCounter;

            _itemApplicator.ItemSelected += OnItemSelected;
            
            _killedEnemyCounter.ResetCounter();
        }

        private void Awake()
        {
            _animationController = GetComponent<CharacterAnimationController>();
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _spriteHolder = GetComponent<CharacterSpriteHolder>();
            _view = GetComponent<CharacterView>();
            _attacker = GetComponent<CharacterAttacker>();

            if (_cameraOnCharacter)
                Camera.main.transform.SetParent(transform);

            _transform = transform;
        }

        private void OnDisable()
        {
            _movementHandler.MovingLeft -= OnMovingLeft;
            _movementHandler.MovingRight -= OnMovingRight;
            _health.Changed -= UpdateHealthView;
            _health.LowHealth -= _spriteHolder.StartPulsing;
            _health.Died -= _spriteHolder.StopPulsing;
            _health.Died -= OnPlayerDied;
            _health.HealthRegainedToNormal -= _spriteHolder.StopPulsing;

            if (_collisionHandler != null)
            {
                _collisionHandler.GotExpPoint -= OnExperienceGained;
                _collisionHandler.GotHeal -= TakeHeal;
            }

            if (_characterLevelController != null)
                _characterLevelController.LeveledUp -= OnLeveledUp;

            _characterLevelController?.Dispose();
            _characterSessionWallet?.Dispose();

            _itemApplicator.ItemSelected -= OnItemSelected;
        }

        private void Update()
        {
            HandleMovementAnimations();
        }

        private void OnExperienceGained(int value)
        {
            UpdateExperienceView(_characterLevelController.CurrentExp);
        }

        private void OnPlayerDied()
        {
            _isDied = true;

            DateTime endGameTime = DateTime.Now;
            TimeSpan gameSession = endGameTime - _gameStart;

            Statistics statistics = new Statistics(_characterLevelController.CurrentExp,
                _characterLevelController.CurrentLevel, _killedEnemyCounter.KilledCounter,
                _characterSessionWallet.CollectedMoney, gameSession, _inventory.GetItemStatisticsList());

            StatisticCollected?.Invoke(statistics);
        }

        private void OnItemSelected(ItemVariations selectedItemVariation)
        {
            Item existingItem = null;

            foreach (var item in _inventory.Items)
            {
                if (item.Data.ItemVariation != selectedItemVariation) continue;
                existingItem = item;
                break;
            }

            if (existingItem != null)
            {
                existingItem.LevelUp();
            }
            else
            {
                Item newItem = _itemsHolder.GetItemByType(selectedItemVariation);

                if (newItem != null)
                {
                    newItem.gameObject.SetActive(true);

                    if (newItem.Data.ItemVariation != ItemVariations.Parfume)
                    {
                        newItem.transform.SetParent(_transform);
                    }

                    if (newItem.Data.ItemVariation == ItemVariations.Backpack)
                    {
                        BackpackItem backpackItem = (BackpackItem)newItem;
                        backpackItem.InvincibilityEnabled += OnInvincibilityEnabled;
                        backpackItem.InvincibilityDisabled += OnInvincibilityDisabled;
                    }

                    if (newItem.Data.ItemVariation == ItemVariations.MultiSlingshot)
                    {
                        MultiSlingshot multiSlingshot = (MultiSlingshot)newItem;
                        multiSlingshot.SetMovementHandler(_movementHandler);
                    }

                    newItem.transform.position = _transform.position;
                    newItem.Initialize(_movementHandler);

                    _inventory.AddItem(newItem);
                }
            }
        }

        private void OnLeveledUp()
        {
            UpdateExperienceView(_characterLevelController.CurrentExp);

            LevelUp?.Invoke();
        }

        public void ActivateCharacter()
        {
            _movementHandler.EnableMovement();
            _movementHandler.SetSpeed(_moveSpeed);
            _attacker.EnableAttack();
            CameraShake.Instance.SetTarget(_transform);
        }

        public void DisableCharacter()
        {
            _attacker.DisableAttack();
            _movementHandler.DisableMovement();
            _movementHandler.SetSpeed(0);
        }

        public void TakeDamage(float damage)
        {
            if (_isInvincible)
                return;

            CameraShake.Instance.ShakeCamera(2, 5, 0.3f);
            _health.TakeDamage(damage);
        }

        private void TakeHeal(int value)
        {
            if (_isInvincible)
                return;

            _health.TakeHeal(value);
        }

        public void Revive()
        {
            _health.TakeHeal(_hp);
            UpdateHealthView(_hp);
        }

        private void OnInvincibilityEnabled()
        {
            _isInvincible = true;
        }

        private void OnInvincibilityDisabled()
        {
            _isInvincible = false;
        }

        private void InitializeCharacterComponents()
        {
            _health = new Health(_hp);

            _inventory = new CharacterInventory();

            _attacker.Initialize(_inventory, _attackCooldown);

            _inventoryUI.DisableAllSlots();
            _inventoryUI.Initialize(_inventory);

            _characterSessionWallet = new CharacterSessionWallet();
            _characterLevelController = new CharacterLevelController();
            _characterSessionWallet.Initialize(_collisionHandler);
            _characterLevelController.Initialize(_collisionHandler);

            _characterLevelController.LeveledUp += OnLeveledUp;
        }

        private void InitializeCharacterData(CharacterData characterData, Dictionary<PerkType, float> perkBonuses)
        {
            _attackPower = characterData.AttackPower + GetPerkBonus(perkBonuses, PerkType.Power);
            _armor = characterData.Armor + GetPerkBonus(perkBonuses, PerkType.Armor);
            _hp = characterData.Hp + GetPerkBonus(perkBonuses, PerkType.MaxHp);
            _hpRegenerationSpeed =
                characterData.HpRegenerationSpeed + GetPerkBonus(perkBonuses, PerkType.HpRegeneration);
            _attackCooldown = characterData.AttackRegenerationSpeed -
                              GetPerkBonus(perkBonuses, PerkType.AttackCooldown);
            _moveSpeed = characterData.MoveSpeed + GetPerkBonus(perkBonuses, PerkType.Speed);

            OnItemSelected(characterData.StartItem.Data.ItemVariation);
            _health.SetMaxHealth(_hp);
            UpdateHealthView(_hp);
        }

        private void HandleMovementAnimations()
        {
            bool isMoving = _movementHandler.IsMoving();
            _animationController.SetWalking(isMoving);

            if (!isMoving) return;
            bool movingLeft = _movementHandler.IsMovingLeft();
            _spriteHolder.FlipSprite(movingLeft);
        }

        private void UpdateHealthView(float currentHealth)
        {
            _view.UpdateHpBar(currentHealth, _hp);
        }

        private void UpdateExperienceView(int currentExp)
        {
            _view.UpdateLevelBar(currentExp, _characterLevelController.RequiredExpForNextLevel);
        }

        private float GetPerkBonus(Dictionary<PerkType, float> perks, PerkType type)
        {
            return perks.GetValueOrDefault(type, 0f);
        }

        private void OnMovingLeft()
        {
            _spriteHolder.FlipSprite(true);
        }

        private void OnMovingRight()
        {
            _spriteHolder.FlipSprite(false);
        }
    }
}