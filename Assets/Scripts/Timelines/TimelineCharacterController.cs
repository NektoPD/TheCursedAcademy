using CharacterLogic.Initializer;
using UnityEngine;
using Zenject;

namespace Timelines
{
    public class TimelineCharacterController : MonoBehaviour
    {
        private CharacterInitializer _initializer;

        [Inject]
        private void Construct(CharacterInitializer characterInitializer)
        {
            _initializer = characterInitializer;
        }

        public void Disable() => _initializer.Character.DisableCharacter();

        public void Enable() => _initializer.Character.ActivateCharacter();
    }
}