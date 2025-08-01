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

        private PerkController _perkController;
        private CharacterFabric _fabric;
        private ItemsHolder _itemsHolder;
        private ItemApplicator _itemApplicator;
        private KilledEnemyCounter _killedEnemyCounter;

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
            CreateCharacter((CharacterData.CharacterType)PlayerPrefs.GetInt(Key));
        }

        public void CreateCharacter(CharacterData.CharacterType type)
        {
            CharacterData chosenData = _characterDatas.FirstOrDefault(data => data.Type == type);

            if (chosenData == null)
                throw new NullReferenceException(nameof(chosenData));

            Dictionary<PerkType, float> finalPerkBonuses = _perkController.GetFinalPerkValues();

            Character characterToSpawn = _fabric.Create();
            characterToSpawn.Construct(chosenData, finalPerkBonuses, _itemsHolder, _itemApplicator,
                _killedEnemyCounter);
            _characterSpawner.Spawn(characterToSpawn);
            PlayerTransform = characterToSpawn.transform;
            CharacterCreated?.Invoke(characterToSpawn);
        }
    }
}