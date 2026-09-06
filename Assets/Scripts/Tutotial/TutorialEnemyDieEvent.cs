using System;
using System.Collections;
using CharacterLogic;
using CharacterLogic.Initializer;
using Timelines;
using UnityEngine;
using Zenject;

namespace Tutorial
{
    public class TutorialEnemyDieEvent : MonoBehaviour
    {
        [SerializeField] private DummyTutorial _dummy;
        [SerializeField] private TutorialTaskController _taskController;
        [SerializeField] private TimelineController _timelineController;
        [SerializeField] private GameObject _cutscene;
        [SerializeField] private TutorialExitTrigger _exitTrigger;
        [SerializeField, Min(0f)] private float _abilityChargeDelay = 8.5f;

        private CharacterInitializer _initializer;
        private Character _character;
        private bool _abilityPhaseStarted;

        public event Action TutorialEnemyDied;

        [Inject]
        private void Construct(CharacterInitializer initializer)
        {
            _initializer = initializer;
            _initializer.CharacterCreated += OnCharacterCreated;
        }

        private void OnEnable()
        {
            if (_dummy != null)
                _dummy.HitsCompleted += StartAbilityPhase;
        }

        private void OnDisable()
        {
            if (_dummy != null)
                _dummy.HitsCompleted -= StartAbilityPhase;
        }

        private void OnDestroy()
        {
            if (_initializer != null)
                _initializer.CharacterCreated -= OnCharacterCreated;

            if (_character != null)
                _character.AbilityUsed -= OnAbilityUsed;
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
            StartCoroutine(EnableAbilityAfterExplanation());
        }

        private IEnumerator EnableAbilityAfterExplanation()
        {
            yield return new WaitForSeconds(_abilityChargeDelay);
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
