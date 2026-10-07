using System;
using CharacterLogic;
using CharacterLogic.Abilities;
using CharacterLogic.Initializer;
using UnityEngine;

namespace Tutorial
{
    public class TutorialFlowController : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private DummyTutorial _dummy;
        [SerializeField] private TutorialExitTrigger _exitTrigger;

        [Header("Phase 1: Auto attack + hit dummy")]
        [SerializeField] private GameObject _phase1Cutscene;

        [Header("Phase 2: Ability explanation")]
        [SerializeField] private GameObject _phase2Cutscene;
        [SerializeField] private GameObject _fireblastCutscene;
        [SerializeField] private GameObject _ragemodeCutscene;
        [SerializeField] private GameObject _poisonThrowCutscene;

        [Header("Final: Congratulations")]
        [SerializeField] private GameObject _finalCutscene;

        private Character _character;

        private void OnEnable()
        {
            _initializer.CharacterCreated += OnCharacterCreated;
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= OnCharacterCreated;

            if (_dummy != null)
                _dummy.HitsCompleted -= OnDummyHitsCompleted;

            if (_character != null)
                _character.AbilityUsed -= OnAbilityUsed;
        }

        private void OnCharacterCreated(Character character)
        {
            _character = character;
            StartPhaseOne();
        }

        private void StartPhaseOne()
        {
            SetActive(_phase1Cutscene, true);
            _dummy.HitsCompleted += OnDummyHitsCompleted;
        }

        private void OnDummyHitsCompleted()
        {
            _dummy.HitsCompleted -= OnDummyHitsCompleted;
            SetActive(_phase1Cutscene, false);
            StartPhaseTwo();
        }

        private void StartPhaseTwo()
        {
            SetActive(_phase2Cutscene, true);
            SetActive(GetAbilityCutscene(_character.CurrentAbilityType), true);

            _character.AbilityUsed += OnAbilityUsed;
            _character.FillAbilityCharge();
        }

        private void OnAbilityUsed()
        {
            _character.AbilityUsed -= OnAbilityUsed;

            SetActive(_phase2Cutscene, false);
            SetActive(GetAbilityCutscene(_character.CurrentAbilityType), false);

            SetActive(_finalCutscene, true);

            if (_exitTrigger != null)
                _exitTrigger.On();
        }

        private GameObject GetAbilityCutscene(AbilityType type)
        {
            return type switch
            {
                AbilityType.Fireblast => _fireblastCutscene,
                AbilityType.Ragemode => _ragemodeCutscene,
                AbilityType.PoisonThrow => _poisonThrowCutscene,
                _ => null
            };
        }

        private void SetActive(GameObject target, bool state)
        {
            if (target != null)
                target.SetActive(state);
        }
    }
}
