using CharacterLogic;
using CharacterLogic.Initializer;
using System;
using Tutorial;
using UnityEngine;
using Zenject;

namespace Timelines
{
    public class TimelineCharacterController : MonoBehaviour
    {
        [SerializeField] private TutorialEnemyDieEvent _tutorialEnemyDieEvent;
        
        private CharacterInitializer _initializer;
        private Character _character;

        [Inject]
        private void Construct(CharacterInitializer characterInitializer)
        {
            _initializer = characterInitializer;
            _initializer.CharacterCreated += Inizialize;
        }

        private void OnEnable()
        {
            _tutorialEnemyDieEvent.TutorialEnemyDied += EnableAfterTutorialEnemyDeath;
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= Inizialize;
            
            _tutorialEnemyDieEvent.TutorialEnemyDied -= EnableAfterTutorialEnemyDeath;
        }

        public void Disable() => _character.DisableCharacter();

        public void Enable() => _character.ActivateCharacter();

        public void EnableAfterTutorialEnemyDeath() => _character.EnableMovement();

        private void Inizialize(Character character) => _character = character;
    }
}