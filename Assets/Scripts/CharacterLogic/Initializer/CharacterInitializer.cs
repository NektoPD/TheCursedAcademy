using System;
using System.Collections.Generic;
using System.Linq;
using CharacterLogic.Spawner;
using PlayerPerksController;
using UnityEngine;
using Zenject;

namespace CharacterLogic.Initializer
{
    public class CharacterInitializer : MonoBehaviour
    {
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private CharacterData[] _characterDatas;
        [SerializeField] private CharacterSpawner _characterSpawner;

        private PerkController _perkController;

        [Inject]
        private void Construct(PerkController perkController)
        {
            _perkController = perkController;
        }

        private void Start()
        {
            CreateCharacter(CharacterData.CharacterType.Girl3);
        }

        public void CreateCharacter(CharacterData.CharacterType type)
        {
            CharacterData chosenData = _characterDatas.FirstOrDefault(data => data.Type == type);

            if (chosenData == null)
                throw new NullReferenceException(nameof(chosenData));
            
            Dictionary<PerkType, float> finalPerkBonuses = _perkController.GetFinalPerkValues();
            
            Character characterToSpawn = Instantiate(_characterPrefab);
            characterToSpawn.Construct(chosenData, finalPerkBonuses);
            _characterSpawner.Spawn(characterToSpawn);
        }
        
    }
}