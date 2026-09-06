using CharacterLogic;
using CharacterLogic.Initializer;
using Tutorial;
using UnityEngine;

namespace Timelines
{
    public class TimelineCharacterController : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private TutorialEnemyDieEvent _tutorialEnemyDieEvent;

        private Character _character;
        private bool _characterEnabled = true;

        private void OnEnable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated += Initialize;

            if (_tutorialEnemyDieEvent != null)
                _tutorialEnemyDieEvent.TutorialEnemyDied += EnableAfterTutorialEnemyDeath;
        }

        private void OnDisable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated -= Initialize;

            if (_tutorialEnemyDieEvent != null)
                _tutorialEnemyDieEvent.TutorialEnemyDied -= EnableAfterTutorialEnemyDeath;
        }

        public void Disable()
        {
            _characterEnabled = false;

            if (_character != null)
                _character.DisableCharacter();
        }

        public void Enable()
        {
            _characterEnabled = true;

            if (_character != null)
                _character.ActivateCharacter();
        }

        public void EnableAfterTutorialEnemyDeath()
        {
            if (_character != null)
                _character.EnableMovement();
        }

        private void Initialize(Character character)
        {
            _character = character;

            if (!_characterEnabled)
                _character.DisableCharacter();
        }
    }
}
