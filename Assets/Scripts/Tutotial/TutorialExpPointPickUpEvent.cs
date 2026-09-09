using CharacterLogic;
using CharacterLogic.Initializer;
using Data;
using UI.FortuneWheel;
using UnityEngine;

namespace Tutorial
{
    public class TutorialExpPointPickUpEvent : MonoBehaviour
    {
        [SerializeField] private TutorialTaskController _taskController;
        [SerializeField] private CharacterInitializer _initializer;
        [SerializeField] private FortuneWheelWindow _fortuneWheelWindow;

        private Character _character;

        private void OnEnable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated += OnCharacterCreated;

            SubscribeToWheel();
        }

        private void OnDisable()
        {
            if (_initializer != null)
                _initializer.CharacterCreated -= OnCharacterCreated;

            if (_character != null)
                _character.LevelUp -= OnLevelUp;

            UnsubscribeFromWheel();
        }

        private void OnCharacterCreated(Character character)
        {
            if (_character != null)
                _character.LevelUp -= OnLevelUp;

            _character = character;
            _character.LevelUp += OnLevelUp;

            if (_fortuneWheelWindow != null)
                _fortuneWheelWindow.Initialize(character.Inventory);
        }

        private void SubscribeToWheel()
        {
            if (_fortuneWheelWindow == null)
                return;

            _fortuneWheelWindow.ItemRewarded += OnItemRewarded;
            _fortuneWheelWindow.GoldRewarded += OnGoldRewarded;
            _fortuneWheelWindow.BuffRewarded += OnBuffRewarded;
        }

        private void UnsubscribeFromWheel()
        {
            if (_fortuneWheelWindow == null)
                return;

            _fortuneWheelWindow.ItemRewarded -= OnItemRewarded;
            _fortuneWheelWindow.GoldRewarded -= OnGoldRewarded;
            _fortuneWheelWindow.BuffRewarded -= OnBuffRewarded;
        }

        private void OnLevelUp()
        {
            _taskController.ShowNextTask();

            if (_fortuneWheelWindow != null)
                _fortuneWheelWindow.OpenWindow();
        }

        private void OnItemRewarded(ItemVisualData item)
        {
            if (_character != null && item != null)
                _character.SelectWheelItem(item.Variation);

            _fortuneWheelWindow.CloseWindow();
        }

        private void OnGoldRewarded(int amount)
        {
            if (_character != null)
                _character.AddWheelGold(amount);

            _fortuneWheelWindow.CloseWindow();
        }

        private void OnBuffRewarded(WheelBuffData buff)
        {
            if (_character != null && buff != null)
                _character.ApplyTemporaryBuff(buff.Type, buff.Multiplier, buff.DurationSeconds);

            _fortuneWheelWindow.CloseWindow();
        }
    }
}
