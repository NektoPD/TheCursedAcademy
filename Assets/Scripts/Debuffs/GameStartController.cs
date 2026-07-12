using System.Collections.Generic;
using CharacterLogic;
using CharacterLogic.Initializer;
using Difficulties;
using UnityEngine;

namespace Debuffs
{
    public class GameStartController : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _characterInitializer;
        [SerializeField] private Difficulty _difficulty;
        [SerializeField] private SlotMachineWindow _slotMachineWindow;

        private Character _character;

        private void OnEnable()
        {
            _characterInitializer.CharacterCreated += OnCharacterCreated;
            _slotMachineWindow.Finished += OnSlotFinished;
        }

        private void OnDisable()
        {
            _characterInitializer.CharacterCreated -= OnCharacterCreated;
            _slotMachineWindow.Finished -= OnSlotFinished;
        }

        private void OnCharacterCreated(Character character)
        {
            _character = character;
            _character.DisableCharacter();
            _slotMachineWindow.OpenUnscaledTime();
        }

        private void OnSlotFinished(IReadOnlyList<DebuffData> debuffs)
        {
            _character.ApplyDebuffs(debuffs);
            _character.ActivateCharacter();
            _slotMachineWindow.CloseWindow();
            _difficulty.StartSpawning();
        }
    }
}
