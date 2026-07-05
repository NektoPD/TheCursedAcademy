using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CameraExtensions;
using CharacterLogic.Abilities;
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
using UI;
using UI.Applicators;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

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
        private const string TutorialSceneName = "Tutorial";
        private const int MaxLevelCoinReward = 30;
        
        [SerializeField] private CharacterInventoryUI _inventoryUI;
        [SerializeField] private bool _cameraOnCharacter;
        [SerializeField] private Canvas _characterCanvas;
        [SerializeField] private float _reviveInvincibilityDuration = 3f;
        [SerializeField] private float _deathZoomResetDuration = 0.15f;
        [SerializeField] private SimpleSpriteAnimator _fireblastActivationEffect;
        [SerializeField] private SimpleSpriteAnimator _ragemodeActivationEffect;
        [SerializeField] private SimpleSpriteAnimator _poisonThrowActivationEffect;
        [SerializeField] private float _deathFadeDuration = 0.7f;
        private Coroutine _deathSequenceCoroutine;
        private Coroutine _reviveInvincibilityCoroutine;
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
        private ItemApplicator _levelUpItemApplicator;
        private ItemApplicatorCurrentOnly _fullInventoryItemApplicator;
        private KilledEnemyCounter _killedEnemyCounter;
        private CharacterSoundController _characterSoundController;
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
        private float _gameStartTime;
        private bool _isTutorial;
        private ItemVariations _stashedItemForChange;
        private float _abilityChargeLevel;
        private AbilityBase _ability;
        private float _baseAttackPower;
        private float _baseArmor;
        public event Action<float, float> HealthChanged;
        public event Action<float, float> Damaged;
        public event Action<float, float> Healed;
        public event Action<Statistics> StatisticCollected;
        public event Action LevelUp;
        public event Action InventoryLimitReached;
        public event Action NewItemAdded;
        public event Action ItemSwapped;
        public event Action MaxLevelReached;
        public event Action AbilityReady;
        public CharacterInventory Inventory => _inventory;
        public bool IsDied => _isDied;

        public void Construct(CharacterData characterData, Dictionary<PerkType, float> perkBonuses,
            ItemsHolder itemsHolder, ItemApplicator levelUpItemApplicator, KilledEnemyCounter killedEnemyCounter,
            CharacterSoundController characterSoundController, ItemApplicatorCurrentOnly fullInventoryItemApplicator = null)
        {
            _characterData = characterData;
            _itemsHolder = itemsHolder;
            _characterSoundController = characterSoundController;
            _collisionHandler = GetComponent<CharacterCollisionHandler>();
            InitializeCharacterComponents();
            InitializeCharacterData(characterData, perkBonuses);
            if (!_isTutorial) ActivateCharacter();
            _animationController.SetAnimatorOverride(characterData.AnimatorController);
            _movementHandler.MovingLeft += OnMovingLeft;
            _movementHandler.MovingRight += OnMovingRight;
            _health.Died += _spriteHolder.StopPulsing;
            _health.Died += OnHealthDied;
            _health.Changed += UpdateHealthView;
            _health.LowHealth += _spriteHolder.StartPulsing;
            _health.HealthRegainedToNormal += _spriteHolder.StopPulsing;
            _collisionHandler.GotExpPoint += OnExperienceGained;
            _collisionHandler.GotHeal += TakeHeal;
            _collisionHandler.GotMoney += _ => OnCoinGet();
            _collisionHandler.GotMagnet += OnXpBoostGet;
            _collisionHandler.GotExpPoint += _ => OnXpGet();
            UpdateExperienceView(_characterLevelController.CurrentExp);
            _levelUpItemApplicator = levelUpItemApplicator;

            if (fullInventoryItemApplicator != null)
                _fullInventoryItemApplicator = fullInventoryItemApplicator;

            _killedEnemyCounter = killedEnemyCounter;
            _levelUpItemApplicator.ItemSelected += OnLevelUpItemSelected;

            if (_fullInventoryItemApplicator != null)
                _fullInventoryItemApplicator.ItemSelected += OnChangeItemSelected;

            _killedEnemyCounter.ResetCounter();
            _gameStartTime = Time.timeSinceLevelLoad;

            InitializeAbility(characterData);
        }

        private void Awake()
        {
            _animationController = GetComponent<CharacterAnimationController>();
            _movementHandler = GetComponent<CharacterMovementHandler>();
            _spriteHolder = GetComponent<CharacterSpriteHolder>();
            _view = GetComponent<CharacterView>();
            _attacker = GetComponent<CharacterAttacker>();
            if (_cameraOnCharacter) Camera.main.transform.SetParent(transform);
            _transform = transform;
            _isTutorial = SceneManager.GetActiveScene().name is TutorialSceneName;
        }

        private void OnDisable()
        {
            _movementHandler.MovingLeft -= OnMovingLeft;
            _movementHandler.MovingRight -= OnMovingRight;
            _health.Changed -= UpdateHealthView;
            _health.LowHealth -= _spriteHolder.StartPulsing;
            _health.Died -= _spriteHolder.StopPulsing;
            _health.Died -= OnHealthDied;
            _health.HealthRegainedToNormal -= _spriteHolder.StopPulsing;
            _inventory.MaxLevelReached -= AddMaxLevelReward;
            
            if (_collisionHandler != null)
            {
                _collisionHandler.GotExpPoint -= OnExperienceGained;
                _collisionHandler.GotHeal -= TakeHeal;
                _collisionHandler.GotMagnet -= OnXpBoostGet;
            }

            if (_characterLevelController != null) _characterLevelController.LeveledUp -= OnLeveledUp;
            _characterLevelController?.Dispose();
            _characterSessionWallet?.Dispose();
            _levelUpItemApplicator.ItemSelected -= OnLevelUpItemSelected;

            if (_fullInventoryItemApplicator != null)
                _fullInventoryItemApplicator.ItemSelected -= OnChangeItemSelected;

            if (_ability != null)
            {
                _ability.ChargeChanged -= UpdateAbilityLevelView;
                _ability.AbilityReady -= OnAbilityReady;
                _view.AbilityButtonPressed -= ActivateAbility;

                if (_ability is RagemodeAbility rage)
                {
                    rage.RageModeStarted -= OnRageModeStarted;
                    rage.RageModeEnded -= OnRageModeEnded;
                }
            }

            if (_killedEnemyCounter != null)
                _killedEnemyCounter.EnemyKilled -= OnEnemyKilledForAbility;
        }

        private void Update()
        {
            HandleMovementAnimations();

            if (_ability != null && _ability.IsReady && Input.GetKeyDown(KeyCode.F))
                ActivateAbility();
        }

        private void OnExperienceGained(int value)
        {
            UpdateExperienceView(_characterLevelController.CurrentExp);
        }

        private void OnCoinGet()
        {
            _characterSoundController.EnableSoundByType(SoundType.Coin);
        }

        private void OnXpGet()
        {
            _characterSoundController.EnableSoundByType(SoundType.XpPoint);
        }

        private void OnXpBoostGet()
        {
            _characterSoundController.EnableSoundByType(SoundType.XpBooster);
        }

        private void OnHealthDied()
        {
            if (_deathSequenceCoroutine != null) return;
            _deathSequenceCoroutine = StartCoroutine(DeathSequenceRoutine());
        }
        
        private IEnumerator DeathSequenceRoutine()
        {
            _isDied = true;
            DisableCharacter();
            CameraShake.Instance?.StopShake();
            _isInvincible = true;
            float previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            if (CameraDeathZoom.Instance != null)
            {
                CameraDeathZoom.Instance.SetTarget(_transform);
                CameraDeathZoom.Instance.PlayDeathZoom();
            }

            Coroutine fade = _spriteHolder.PlayDeathFade(this);
            yield return fade;
            CameraDeathZoom.Instance?.ResetZoom(_deathZoomResetDuration);
            Time.timeScale = previousTimeScale;
            OnPlayerDied();
            _deathSequenceCoroutine = null;
        }

        private void OnPlayerDied()
        {
            _isDied = true;
            _characterSoundController.EnableSoundByType(SoundType.GameOver);

            float liveSeconds = Time.timeSinceLevelLoad - _gameStartTime;
            if (liveSeconds < 0f) liveSeconds = 0f;

            TimeSpan gameSession = TimeSpan.FromSeconds(liveSeconds);

            Statistics statistics = new Statistics(_characterLevelController.CurrentExp,
                _characterLevelController.CurrentLevel, _killedEnemyCounter.KilledCounter,
                _characterSessionWallet.CollectedMoney, gameSession, _inventory.GetItemStatisticsList());
            StatisticCollected?.Invoke(statistics);
        }

        private void OnLevelUpItemSelected(ItemVariations selectedItemVariation)
        {
            Item existingItem = _inventory.Items.FirstOrDefault(i => i.Data.ItemVariation == selectedItemVariation);

            if (existingItem != null)
            {
                if (existingItem.CurrentLevel >= existingItem.Data.MaxLevel)
                {
                    MaxLevelReached?.Invoke();
                    return;
                }

                existingItem.LevelUp();
                NewItemAdded?.Invoke();
            }
            else
            {
                if (!_inventory.CanFitOneMoreItem())
                {
                    _stashedItemForChange = selectedItemVariation;
                    InventoryLimitReached?.Invoke();
                    return;
                }

                SetupNewItem(selectedItemVariation);
                NewItemAdded?.Invoke();
            }
        }


        private void OnChangeItemSelected(ItemVariations selectedItemForChangeVariation)
        {
            _inventory.RemoveItem(selectedItemForChangeVariation);
            SetupNewItem(_stashedItemForChange);
            ItemSwapped?.Invoke();
        }

        private void SetupNewItem(ItemVariations selectedItemVariation)
        {
            Item newItem = _itemsHolder.GetItemByType(selectedItemVariation);
            
            if (newItem == null)
                return;

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
            newItem.Initialize(_movementHandler, _characterSoundController);
            _inventory.AddItem(newItem);
        }

        private void OnLeveledUp()
        {
            UpdateExperienceView(_characterLevelController.CurrentExp);
            _characterSoundController.EnableSoundByType(SoundType.LevelUp);
            LevelUp?.Invoke();
        }

        public void ActivateCharacter()
        {
            _movementHandler.EnableMovement();
            _movementHandler.SetSpeed(_moveSpeed);
            _attacker.EnableAttack();
            CameraShake.Instance.SetTarget(_transform);
            if (_characterCanvas != null) _characterCanvas.gameObject.SetActive(true);
        }

        public void DisableCharacter()
        {
            _attacker.DisableAttack();
            _movementHandler.DisableMovement();
            _movementHandler.SetSpeed(0);
            if (_characterCanvas != null) _characterCanvas.gameObject.SetActive(false);
        }

        public void EnableMovement()
        {
            _movementHandler.EnableMovement();
            _movementHandler.SetSpeed(_moveSpeed);
            if (_characterCanvas != null) _characterCanvas.gameObject.SetActive(true);
        }

        public void TakeDamage(float damage)
        {
            if (_isInvincible) return;
            CameraShake.Instance.ShakeCamera(2, 5, 0.3f);
            _health.TakeDamage(damage);
            _characterSoundController.EnableSoundByType(SoundType.Hit);
            Damaged?.Invoke(_health.CurrentHealth, _hp);
            HealthChanged?.Invoke(_health.CurrentHealth, _hp);
        }

        private void TakeHeal(int value)
        {
            if (_isInvincible) return;
            _health.TakeHeal(value);
            _characterSoundController.EnableSoundByType(SoundType.Heal);
            Healed?.Invoke(_health.CurrentHealth, _hp);
            HealthChanged?.Invoke(_health.CurrentHealth, _hp);
        }

        private void AddMaxLevelReward()
        {
            _characterSoundController.EnableSoundByType(SoundType.MaxLevel);
            _characterSessionWallet.AddMoney(MaxLevelCoinReward);
        }

        public void Revive()
        {
            _isDied = false;
            Time.timeScale = 1f;
            _health.TakeHeal(_hp);
            UpdateHealthView(_hp);
            _characterSoundController.EnableSoundByType(SoundType.Heal);
            Healed?.Invoke(_health.CurrentHealth, _hp);
            HealthChanged?.Invoke(_health.CurrentHealth, _hp);
            ActivateCharacter();
            _spriteHolder.ResetVisual();
            if (_reviveInvincibilityCoroutine != null) StopCoroutine(_reviveInvincibilityCoroutine);
            _reviveInvincibilityCoroutine = StartCoroutine(ReviveInvincibilityRoutine());
        }

        private IEnumerator ReviveInvincibilityRoutine()
        {
            _isInvincible = true;
            _attacker.DisableAttack();
            _spriteHolder.SetInvincibleVisual(true);
            yield return new WaitForSeconds(_reviveInvincibilityDuration);
            _isInvincible = false;
            _attacker.EnableAttack();
            _spriteHolder.SetInvincibleVisual(false);
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
            _inventory = new CharacterInventory(_characterData.InventoryLimit);
            _attacker.Initialize(_inventory, _attackCooldown);
            _inventoryUI.DisableAllSlots();
            _inventoryUI.Initialize(_inventory);
            _inventory.MaxLevelReached += AddMaxLevelReward;
            _characterSessionWallet = new CharacterSessionWallet();
            _characterLevelController = new CharacterLevelController();
            _characterSessionWallet.Initialize(_collisionHandler);
            _characterLevelController.Initialize(_collisionHandler);
            _characterLevelController.LeveledUp += OnLeveledUp;
        }

        private void InitializeCharacterData(CharacterData characterData, Dictionary<PerkType, float> perkBonuses)
        {
            _attackPower = characterData.AttackPower * GetPerkBonus(perkBonuses, PerkType.Power);
            _armor = characterData.Armor * GetPerkBonus(perkBonuses, PerkType.Armor);
            _hp = characterData.Hp * GetPerkBonus(perkBonuses, PerkType.MaxHp);
            _hpRegenerationSpeed =
                characterData.HpRegenerationSpeed * GetPerkBonus(perkBonuses, PerkType.HpRegeneration);
            _attackCooldown = characterData.AttackRegenerationSpeed *
                              GetPerkBonus(perkBonuses, PerkType.AttackCooldown);
            _moveSpeed = characterData.MoveSpeed * GetPerkBonus(perkBonuses, PerkType.Speed);
            SetupNewItem(characterData.StartItem.Data.ItemVariation);
            _health.SetMaxHealth(_hp);
            _view.SetHeroImage(characterData.HeroSprite);
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

        private void UpdateAbilityLevelView(float currentValue, float maxValue)
        {
            _view.UpdateAbilityLevelBar(currentValue, maxValue);
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

        private void InitializeAbility(CharacterData characterData)
        {
            if (characterData.AbilityConfig == null) return;

            AbilityConfig config = characterData.AbilityConfig;
            _abilityChargeLevel = config.KillsToCharge;

            AbilityBase abilityPrefab = config.Type switch
            {
                AbilityType.Fireblast => gameObject.AddComponent<FireblastAbility>(),
                AbilityType.Ragemode => gameObject.AddComponent<RagemodeAbility>(),
                AbilityType.PoisonThrow => gameObject.AddComponent<PoisonThrowAbility>(),
                _ => null
            };

            if (abilityPrefab == null) return;

            _ability = abilityPrefab;
            _ability.Initialize(config, _transform, _characterSoundController);

            SimpleSpriteAnimator activationEffect = config.Type switch
            {
                AbilityType.Fireblast => _fireblastActivationEffect,
                AbilityType.Ragemode => _ragemodeActivationEffect,
                AbilityType.PoisonThrow => _poisonThrowActivationEffect,
                _ => null
            };

            if (activationEffect != null)
                _ability.SetActivationEffect(activationEffect);

            if (_ability is PoisonThrowAbility poison)
                poison.SetMovementHandler(_movementHandler);

            if (_ability is RagemodeAbility rage)
            {
                _baseAttackPower = _attackPower;
                _baseArmor = _armor;
                rage.RageModeStarted += OnRageModeStarted;
                rage.RageModeEnded += OnRageModeEnded;
            }

            _ability.ChargeChanged += UpdateAbilityLevelView;
            _ability.AbilityReady += OnAbilityReady;
            _killedEnemyCounter.EnemyKilled += OnEnemyKilledForAbility;
            _view.AbilityButtonPressed += ActivateAbility;

            _view.UpdateAbilityLevelBar(0f, _abilityChargeLevel);
        }

        private void OnEnemyKilledForAbility()
        {
            _ability?.AddCharge();
        }

        private void OnAbilityReady()
        {
            _view.ShowAbilityReady();
            AbilityReady?.Invoke();
        }

        public void ActivateAbility()
        {
            if (_ability == null || !_ability.IsReady) return;
            _ability.Activate();
            _view.HideAbilityUI();
        }

        private void OnRageModeStarted(float damageMult)
        {
            _attackPower = _baseAttackPower * damageMult;
        }

        private void OnRageModeEnded()
        {
            _attackPower = _baseAttackPower;
        }
    }
}