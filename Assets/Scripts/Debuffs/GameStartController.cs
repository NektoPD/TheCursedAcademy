using System.Collections.Generic;
using CharacterLogic;
using CharacterLogic.Initializer;
using Difficulties;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public class GameStartController : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _characterInitializer;
        [SerializeField] private Difficulty _difficulty;
        [SerializeField] private EventStarter _eventStarter;
        [SerializeField] private UI.TimeView _timeView;
        [SerializeField] private SlotMachineWindow _slotMachineWindow;
        [SerializeField] private CurseRevealOverlay _curseRevealOverlay;
        [SerializeField] private Button _pauseButton;
        [SerializeField, Min(0f)] private float _negativeEffectIncreasePercent = 25f;
        [SerializeField, Min(0f)] private float _coinBonusPercent = 25f;

        private Character _character;
        private IReadOnlyList<DebuffRoll> _debuffs;
        private bool _choiceHandled;
        private bool _pauseHeld;

        public bool IsGameStarted { get; private set; }

        private void Awake()
        {
            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _characterInitializer.CharacterCreated += OnCharacterCreated;
            _slotMachineWindow.Finished += OnSlotFinished;
            _curseRevealOverlay.Confirmed += OnCurseRevealConfirmed;
            _curseRevealOverlay.Closed += OnCurseRevealClosed;
        }

        private void OnDisable()
        {
            _characterInitializer.CharacterCreated -= OnCharacterCreated;
            _slotMachineWindow.Finished -= OnSlotFinished;
            _curseRevealOverlay.Confirmed -= OnCurseRevealConfirmed;
            _curseRevealOverlay.Closed -= OnCurseRevealClosed;
            _slotMachineWindow.Closed -= OnSlotClosed;
            ReleasePause();
        }

        private void OnCharacterCreated(Character character)
        {
            _character = character;
            IsGameStarted = false;
            _choiceHandled = false;
            _pauseHeld = true;
            GamePauseController.HoldPause(muteAudio: false);
            _character.DisableCharacter();
            _slotMachineWindow.OpenUnscaledTime();
        }

        private void OnSlotFinished(IReadOnlyList<DebuffRoll> debuffs)
        {
            _debuffs = debuffs;
            _slotMachineWindow.Closed += OnSlotClosed;
            _slotMachineWindow.CloseUnscaledTime();
        }

        private void OnSlotClosed()
        {
            _slotMachineWindow.Closed -= OnSlotClosed;
            _curseRevealOverlay.Show(_debuffs, _negativeEffectIncreasePercent, _coinBonusPercent);
        }

        private void OnCurseRevealConfirmed(bool dealAccepted)
        {
            if (_choiceHandled || _character == null)
                return;

            _choiceHandled = true;

            float negativeEffectMultiplier = dealAccepted
                ? 1f + _negativeEffectIncreasePercent / 100f
                : 1f;
            float coinMultiplier = dealAccepted
                ? 1f + _coinBonusPercent / 100f
                : 1f;

            _character.ApplyDebuffs(_debuffs, negativeEffectMultiplier);
            _character.SetCoinMultiplier(coinMultiplier);
        }

        private void OnCurseRevealClosed()
        {
            if (!_choiceHandled || !_pauseHeld || _character == null)
                return;

            ReleasePause();
            _character.ActivateCharacter();
            IsGameStarted = true;
            _difficulty.StartSpawning();

            if (_eventStarter != null)
                _eventStarter.StartSpawning();

            if (_timeView != null)
                _timeView.StartTimer();

            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(true);
        }

        private void ReleasePause()
        {
            if (!_pauseHeld)
                return;

            _pauseHeld = false;
            GamePauseController.ReleasePause();
        }
    }
}
