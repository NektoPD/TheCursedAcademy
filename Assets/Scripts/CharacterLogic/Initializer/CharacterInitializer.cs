using System;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic.Data;
using CharacterLogic.Spawner;
using Installers;
using Items.ItemHolder;
using PlayerPerksController;
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

        public Transform PlayerTransform { get; private set; }

        public Character Character { get; private set; }

        [Inject]
        private void Construct(PerkController perkController, CharacterFabric fabric, ItemsHolder itemsHolder)
        {
            _perkController = perkController;
            _fabric = fabric;

            Debug.Log(itemsHolder);
            _itemsHolder = itemsHolder;
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
            characterToSpawn.Construct(chosenData, finalPerkBonuses, _itemsHolder);
            _characterSpawner.Spawn(characterToSpawn);
            PlayerTransform = characterToSpawn.transform;
            Character = characterToSpawn;
        }
    }
}