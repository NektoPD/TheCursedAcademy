using System;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic.Data;
using CharacterLogic.Spawner;
using EnemyLogic;
using Installers;
using Items.ItemHolder;
using PlayerPerksController;
using UI.Applicators;
using UnityEngine;
using Zenject;

namespace CharacterLogic.Initializer
{
    public class CharacterInitializer : MonoBehaviour
    {
        private const string Key = "CharacterId";
        [SerializeField] private CharacterData[] _characterDatas;
        [SerializeField] private CharacterSpawner _characterSpawner;
        [SerializeField] private CharacterSoundController _characterSoundController;
        [SerializeField] private ItemApplicator _fullInventoryItemApplicator;
        private PerkController _perkController;
        private CharacterFabric _fabric;
        private ItemsHolder _itemsHolder;
        private ItemApplicator _itemApplicator;
        private KilledEnemyCounter _killedEnemyCounter;
        private bool _isTutorial;
        public bool WasRevivedThisSession { get; private set; }
        public event Action<Character> CharacterCreated;
        public Transform PlayerTransform { get; private set; }

        [Inject]
        private void Construct(PerkController perkController, CharacterFabric fabric, ItemsHolder itemsHolder,
            ItemApplicator itemApplicator, KilledEnemyCounter enemyCounter)
        {
            _perkController = perkController;
            _fabric = fabric;
            _itemsHolder = itemsHolder;
            _itemApplicator = itemApplicator;
            _killedEnemyCounter = enemyCounter;
        }

        private void Start()
        {
            WasRevivedThisSession = false;
            CreateCharacter((CharacterData.CharacterType)PlayerPrefs.GetInt(Key));
        }

        public void MarkRevived()
        {
            WasRevivedThisSession = true;
        }

        public void CreateCharacter(CharacterData.CharacterType type)
        {
            CharacterData chosenData = _characterDatas.FirstOrDefault(data => data.Type == type);
            if (chosenData == null) throw new NullReferenceException(nameof(chosenData));
            Dictionary<PerkType, float> finalPerkBonuses = _perkController.GetFinalPerkValues();
            Character characterToSpawn = _fabric.Create();
            if (_isTutorial) characterToSpawn.DisableCharacter();
            characterToSpawn.Construct(chosenData, finalPerkBonuses, _itemsHolder, _itemApplicator, _killedEnemyCounter,
                _characterSoundController, _fullInventoryItemApplicator);
            _characterSpawner.Spawn(characterToSpawn);
            PlayerTransform = characterToSpawn.transform;
            CharacterCreated?.Invoke(characterToSpawn);
        }
    }
}