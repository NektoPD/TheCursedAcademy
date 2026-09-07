using System;
using System.Collections;
using CharacterLogic;
using CharacterLogic.Initializer;
using Timelines;
using UI;
using UI.Animation;
using UI.FortuneWheel;
using UnityEngine;

namespace Tutorial
{
    public class TutorialEnemyDieEvent : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private DummyTutorial _dummy;
        [SerializeField] private TutorialTaskController _taskController;
        [SerializeField] private TimelineController _timelineController;
        [SerializeField] private GameObject _cutscene;
        [SerializeField] private TutorialExitTrigger _exitTrigger;
        [SerializeField] private WindowAnimation _showLevelUpTraining;
        [SerializeField] private LevelUpWindow _levelUpMain;
        [SerializeField] private FortuneWheelWindow _fortuneWheelDemo;
        [SerializeField, Min(0f)] private float _abilityChargeDelay = 5f;
        [SerializeField, Min(0.1f)] private float _abilityChargeDuration = 0.75f;
        [SerializeField, Min(0f)] private float _characterReleaseDelay = 8.5f;

        private Character _character;
        private Coroutine _abilityRoutine;
        private bool _abilityPhaseStarted;

        public event Action TutorialEnemyDied;

        private void OnEnable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated += OnCharacterCreated;

            if (_dummy != null)
                _dummy.HitsCompleted += StartAbilityPhase;

            if (_showLevelUpTraining != null)
                _showLevelUpTraining.Closed += ShowLevelUpMain;

            if (_levelUpMain != null)
                _levelUpMain.Closed += HideFortuneWheelDemo;
        }

        private void OnDisable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated -= OnCharacterCreated;

            if (_dummy != null)
                _dummy.HitsCompleted -= StartAbilityPhase;

            if (_showLevelUpTraining != null)
                _showLevelUpTraining.Closed -= ShowLevelUpMain;

            if (_levelUpMain != null)
                _levelUpMain.Closed -= HideFortuneWheelDemo;

            if (_character != null)
                _character.AbilityUsed -= OnAbilityUsed;

            if (_abilityRoutine != null)
            {
                StopCoroutine(_abilityRoutine);
                _abilityRoutine = null;
            }
        }

        private void OnCharacterCreated(Character character)
        {
            _character = character;

            if (_levelUpMain != null)
                _levelUpMain.Initialize(character.Inventory);
        }

        private void StartAbilityPhase()
        {
            if (_abilityPhaseStarted || _character == null)
                return;

            _abilityPhaseStarted = true;
            _dummy.HitsCompleted -= StartAbilityPhase;
            _timelineController.StartCutscene(_cutscene.name);
            _taskController.ShowNextTask();
            _character.AbilityUsed += OnAbilityUsed;
            _abilityRoutine = StartCoroutine(EnableAbilityAfterExplanation());
        }

        private IEnumerator EnableAbilityAfterExplanation()
        {
            yield return new WaitForSeconds(_abilityChargeDelay);
            _character.FillAbilityCharge(_abilityChargeDuration);

            float remainingDelay = _characterReleaseDelay - _abilityChargeDelay;

            if (remainingDelay > 0f)
                yield return new WaitForSeconds(remainingDelay);

            _abilityRoutine = null;
            TutorialEnemyDied?.Invoke();
        }

        private void OnAbilityUsed()
        {
            _character.AbilityUsed -= OnAbilityUsed;
            _taskController.ShowNextTask();

            if (_showLevelUpTraining != null)
            {
                _showLevelUpTraining.Open();
                _showLevelUpTraining.StopTime();
            }

            if (_exitTrigger != null)
                _exitTrigger.On();
        }

        private void ShowLevelUpMain()
        {
            if (_levelUpMain != null)
                _levelUpMain.OpenWindow();

            if (_fortuneWheelDemo != null && _character != null)
                _fortuneWheelDemo.PlayDemo(_character.Inventory);
        }

        private void HideFortuneWheelDemo()
        {
            if (_fortuneWheelDemo != null)
                _fortuneWheelDemo.StopDemo();
        }
    }
}
