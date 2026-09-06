using System;
using System.Collections;
using CharacterLogic;
using CharacterLogic.Initializer;
using Timelines;
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
        [SerializeField, Min(0f)] private float _abilityChargeDelay = 8.5f;

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
        }

        private void OnDisable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated -= OnCharacterCreated;

            if (_dummy != null)
                _dummy.HitsCompleted -= StartAbilityPhase;

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
            _abilityRoutine = null;
            _character.FillAbilityCharge();
            TutorialEnemyDied?.Invoke();
        }

        private void OnAbilityUsed()
        {
            _character.AbilityUsed -= OnAbilityUsed;
            _taskController.ShowNextTask();

            if (_exitTrigger != null)
                _exitTrigger.On();
        }
    }
}
