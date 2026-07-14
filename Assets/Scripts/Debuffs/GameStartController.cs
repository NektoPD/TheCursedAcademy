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
        [SerializeField] private CurseRevealOverlay _curseRevealOverlay;

        private Character _character;
        private IReadOnlyList<DebuffData> _debuffs;

        private void OnEnable()
        {
            _characterInitializer.CharacterCreated += OnCharacterCreated;
            _slotMachineWindow.Finished += OnSlotFinished;
            _curseRevealOverlay.Confirmed += OnCurseRevealConfirmed;
        }

        private void OnDisable()
        {
            _characterInitializer.CharacterCreated -= OnCharacterCreated;
            _slotMachineWindow.Finished -= OnSlotFinished;
            _curseRevealOverlay.Confirmed -= OnCurseRevealConfirmed;
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
            _debuffs = debuffs;
            _slotMachineWindow.Closed += OnSlotClosed;
            _slotMachineWindow.CloseUnscaledTime();
        }

        private void OnSlotClosed()
        {
            _slotMachineWindow.Closed -= OnSlotClosed;
            _curseRevealOverlay.Show(_debuffs);
        }

        private void OnCurseRevealConfirmed()
        {
            _character.ActivateCharacter();
            _difficulty.StartSpawning();
        }
    }
}
