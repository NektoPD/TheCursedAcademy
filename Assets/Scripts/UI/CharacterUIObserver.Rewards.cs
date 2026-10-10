using System.Collections.Generic;
using System.Linq;
using Items.BaseClass;
using UI.FortuneWheel;
using Utils;

namespace UI
{
    public partial class CharacterUIObserver
    {
        private readonly List<WheelReward> _wheelRewards = new();
        private int _nextRewardIndex;
        private bool _isGrantingRewards;
        private bool _waitingForItemWindow;
        private int _pendingLevelUps;
        private bool _rewardPauseHeld;
        private bool _swapPauseHeld;

        private void LevelUp()
        {
            _pendingLevelUps++;

            if (_rewardPauseHeld)
                return;

            HoldRewardPause();
            OpenPendingLevelUp();
        }

        private void OpenPendingLevelUp()
        {
            if (_pendingLevelUps <= 0)
                return;

            _pendingLevelUps--;
            if (_pauseButton != null)
                _pauseButton.interactable = false;

            if (IsWindowActive(_fortuneWheelWindow))
                _fortuneWheelWindow.PlayNextSpin();
            else
                _fortuneWheelWindow.OpenUnscaledTime();
        }

        private void OnWheelItemRewarded(Data.ItemVisualData item)
        {
            if (item != null)
                _wheelRewards.Add(WheelReward.CreateItem(item));
        }

        private void OnWheelGoldRewarded(int amount)
        {
            _wheelRewards.Add(WheelReward.CreateGold(amount));
        }

        private void OnWheelBuffRewarded(FortuneWheel.WheelBuffData buff)
        {
            if (buff != null)
                _wheelRewards.Add(WheelReward.CreateBuff(buff));
        }

        private void OnWheelFinished()
        {
            if (_pendingLevelUps > 0)
            {
                OpenPendingLevelUp();
                return;
            }

            _fortuneWheelWindow.CloseUnscaledTime();

            if (_wheelRewards.Count > 1)
            {
                _rewardPopup.ShowRewards(_wheelRewards);
                return;
            }

            if (_wheelRewards.Count == 0)
                return;

            WheelReward reward = _wheelRewards[0];
            switch (reward.Type)
            {
                case WheelRewardType.Item:
                    Item existingItem = _character.Inventory.Items.FirstOrDefault(
                        item => item.VisualData.Variation == reward.Item.Variation);
                    _rewardPopup.ShowItem(reward.Item, existingItem);
                    break;
                case WheelRewardType.Gold:
                    _rewardPopup.ShowGold(reward.GoldAmount);
                    break;
                case WheelRewardType.Buff:
                    _rewardPopup.ShowBuff(reward.Buff);
                    break;
            }
        }

        private void OnWheelClosed()
        {
            TryCompleteRewardBatch();
        }

        private void OnRewardPopupConfirmed()
        {
            if (!_rewardPauseHeld || _isGrantingRewards)
                return;

            _isGrantingRewards = true;
            _rewardPopup.CloseUnscaledTime();
        }

        private void GrantNextRewards()
        {
            if (!_isGrantingRewards || _waitingForItemWindow || _character == null)
                return;

            while (_nextRewardIndex < _wheelRewards.Count)
            {
                WheelReward reward = _wheelRewards[_nextRewardIndex];
                _nextRewardIndex++;

                switch (reward.Type)
                {
                    case WheelRewardType.Item:
                        _character.SelectWheelItem(reward.Item.Variation);
                        break;
                    case WheelRewardType.Gold:
                        _character.AddWheelGold(reward.GoldAmount);
                        break;
                    case WheelRewardType.Buff:
                        _character.ApplyTemporaryBuff(reward.Buff.Type, reward.Buff.Multiplier,
                            reward.Buff.DurationSeconds);
                        break;
                }

                if (_waitingForItemWindow)
                    return;
            }

            TryCompleteRewardBatch();
        }

        private void TryCompleteRewardBatch()
        {
            if (!_rewardPauseHeld || IsWindowActive(_fortuneWheelWindow)
                || IsWindowActive(_rewardPopup) || IsWindowActive(_inventoryFullWindow)
                || IsWindowActive(_itemMaxLevelReachedWindow))
                return;

            if (_wheelRewards.Count > 0 && (!_isGrantingRewards || _nextRewardIndex < _wheelRewards.Count))
                return;

            _wheelRewards.Clear();
            _nextRewardIndex = 0;
            _isGrantingRewards = false;
            _waitingForItemWindow = false;

            if (_pendingLevelUps > 0)
            {
                OpenPendingLevelUp();
                return;
            }

            _rewardPauseHeld = false;
            GameTimeScale.SetPauseActive(false);
        }

        private void HoldRewardPause()
        {
            if (_pauseButton != null)
                _pauseButton.interactable = false;

            if (_rewardPauseHeld)
                return;

            _rewardPauseHeld = true;
            GameTimeScale.SetPauseActive(true);
        }

        private void OnRewardPopupClosed()
        {
            if (_isGrantingRewards)
                GrantNextRewards();
            else
                TryCompleteRewardBatch();
        }

        private void OnInventoryFullWindowClosed()
        {
            if (!_swapPauseHeld)
                return;

            _swapPauseHeld = false;

            if (_isGrantingRewards)
            {
                _waitingForItemWindow = false;
                GrantNextRewards();
                return;
            }

            GameTimeScale.SetPauseActive(false);
        }

        private void OnMaxLevelReachedWindowClosed()
        {
            if (_isGrantingRewards)
            {
                _waitingForItemWindow = false;
                GrantNextRewards();
            }
            else
            {
                TryCompleteRewardBatch();
            }
        }

        private void InventoryLimitReached()
        {
            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(false);

            if (_isGrantingRewards)
                _waitingForItemWindow = true;
            else if (_rewardPopup != null)
                _rewardPopup.CloseUnscaledTime();

            _swapPauseHeld = true;
            _inventoryFullWindow.OpenUnscaledTime();
        }
    }
}
