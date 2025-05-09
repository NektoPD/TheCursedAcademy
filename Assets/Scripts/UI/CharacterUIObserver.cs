using CharacterLogic;
using CharacterLogic.Initializer;
using StatistiscSystem;
using UnityEngine;

namespace UI
{
    public class CharacterUIObserver : MonoBehaviour
    {
        [SerializeField] private LevelUpWindow _levelUpWindow;
        [SerializeField] private StatisticsApplicator _applicator;
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private ExitToMenu _exit;
        [SerializeField] private Reviver _reviver;

        private Character _character;

        private void OnEnable()
        {
            _initializer.CharacterCreated += Inizialize;
        }

        private void OnDisable()
        {
            _initializer.CharacterCreated -= Inizialize;

            if (_character == null)
                return;

            _character.StatisticCollected -= StatisticApplicate;
            _character.LevelUp -= LevelUp;
        }

        private void Inizialize(Character character)
        {
            _character = character;
            _reviver.Inizialize(character);
            _character.StatisticCollected += StatisticApplicate;
            _character.LevelUp += LevelUp;
        }

        private void LevelUp() => _levelUpWindow.OpenWindow();

        private void StatisticApplicate(Statistics statistics)
        {
            _applicator.Applicate(statistics);
            _exit.SetCoins(statistics.Coins);
        }
    }
}