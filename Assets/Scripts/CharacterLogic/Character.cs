using System;
using System.Collections.Generic;
using CharacterLogic.Data;
using CharacterLogic.InputHandler;
using HealthSystem;
using InventorySystem;
using Items.BaseClass;
using Items.Enums;
using Items.ItemHolder;
using Items.ItemVariations;
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
    public class Character : MonoBehaviour, IDamageable, IStatisticsTransmitter
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

        private float _attackPower;
        private float _armor;
        private float _hp;
        private float _hpRegenerationSpeed;
        private float _attackCooldown;
        private float _moveSpeed;
        private Item _startItem;
        private bool _isInvincible = false;
        private Transform _transform;

        public event Action<Statistics> StatisticCollected;
        public event Action LevelUp;

        public void Construct(CharacterData characterData, Dictionary<PerkType, float> perkBonuses,
            ItemsHolder itemsHolder, ItemApplicator itemApplicator)
        {
            _itemsHolder = itemsHolder;

            _collisionHandler = GetComponent<CharacterCollisionHandler>();

            InitializeCharacterData(characterData, perkBonuses);
            InitializeCharacterComponents();
            ActivateCharacter();

            _animationController.SetAnimatorOverride(characterData.AnimatorController);

            _movementHandler.MovingLeft += OnMovingLeft;
            _movementHandler.MovingRight += OnMovingRight;
            _health.Changed += UpdateHealthView;

            _collisionHandler.GotExpPoint += OnExperienceGained;

            UpdateExperienceView(_characterLevelController.CurrentExp);

            _itemApplicator = itemApplicator;

            _itemApplicator.ItemSelected += OnItemSelected;
        }

        private void OnExperienceGained(int value)
        {
            UpdateExperienceView(_characterLevelController.CurrentExp);
        }

        private void OnItemSelected(ItemVariations selectedItemVariation)
        {
            Item existingItem = null;

            foreach (var item in _inventory.Items)
            {
                if (item.Data.ItemVariation == selectedItemVariation)
                {
                    existingItem = item;
                    break;
                }
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

                    newItem.transform.position = transform.position;
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

            if (_collisionHandler != null)
                _collisionHandler.GotExpPoint -= OnExperienceGained;

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

        public void ActivateCharacter()
        {
            _movementHandler.EnableMovement();
            _movementHandler.SetSpeed(_moveSpeed);
            _attacker.EnableAttack();
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

            _health.TakeDamage(damage);
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
            UpdateHealthView(_hp);

            _inventory = new CharacterInventory();

            _attacker.Initialize(_inventory, _attackCooldown);

            _inventoryUI.DisableAllSlots();
            _inventoryUI.Initialize(_inventory);

            _inventory.AddItem(_startItem);

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

            _startItem = _itemsHolder.GetItemByType(characterData.StartItem.Data.ItemVariation);

            _startItem.gameObject.SetActive(true);

            if (_startItem.Data.ItemVariation != ItemVariations.Parfume)
            {
                _startItem.transform.SetParent(_transform);
            }

            if (_startItem.Data.ItemVariation == ItemVariations.Backpack)
            {
                BackpackItem backpackItem = (BackpackItem)_startItem;

                backpackItem.InvincibilityEnabled += OnInvincibilityEnabled;
                backpackItem.InvincibilityDisabled += OnInvincibilityDisabled;
            }

            _startItem.transform.position = transform.position;
            _startItem.Initialize(_movementHandler);
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