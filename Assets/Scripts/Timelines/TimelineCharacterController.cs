using CharacterLogic;
using CharacterLogic.Initializer;
using System;
using UnityEngine;
using Zenject;

namespace Timelines
{
    public class TimelineCharacterController : MonoBehaviour
    {
        private CharacterInitializer _initializer;
        private Character _character;

        [Inject]
        private void Construct(CharacterInitializer characterInitializer)
        {
            _initializer = characterInitializer;
            _initializer.CharacterCreated += Inizialize;
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= Inizialize;
        }

        public void Disable() => _character.DisableCharacter();

        public void Enable() => _character.ActivateCharacter();

        private void Inizialize(Character character) => _character = character;
    }
}