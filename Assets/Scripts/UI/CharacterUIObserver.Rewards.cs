using System.Collections.Generic;
using System.Linq;
using Items.BaseClass;
using Items.Stats;
using UI.FortuneWheel;
using UnityEngine;
using Utils;

namespace UI
{
    public partial class CharacterUIObserver
    {
        private readonly List<WheelReward> _wheelRewards = new();
        private IReadOnlyDictionary<PerkType, float> _wheelRewardBaseStats;
        private int _nextRewardIndex;
        private int _pendingNewItemRewardIndex = -1;
        private bool _isGrantingRewards;
        private bool _waitingForItemWindow;
        private bool _waitingForWheelClose;
        private int _pendingLevelUps;
        private bool _rewardPauseHeld;
        private bool _swapPauseHeld;

        private void LevelUp()
        {
            _pendingLevelUps++;

            if (_rewardPauseHeld || _swapPauseHeld)
                return;

            HoldRewardPause();
            OpenPendingLevelUp();
        }

        private void OpenPendingLevelUp()
        {
            if (_pendingLevelUps <= 0 || _swapPauseHeld)
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
            if (_pendingLevelUps > 0 && !_swapPauseHeld)
            {
                OpenPendingLevelUp();
                return;
            }
            
            _wheelRewardBaseStats = _character.GetStatsSnapshot();
            _isGrantingRewards = true;
            _waitingForWheelClose = true;
            _fortuneWheelWindow.CloseUnscaledTime();
        }

        private void OnWheelClosed()
        {
            _waitingForWheelClose = false;
            if (_isGrantingRewards)
                GrantNextRewards();
            else
                TryCompleteRewardBatch();
        }

        private void OnRewardPopupConfirmed()
        {
            if (!_rewardPauseHeld)
                return;

            _rewardPopup.CloseUnscaledTime();
        }

        private void GrantNextRewards()
        {
            if (!_isGrantingRewards || _waitingForItemWindow || _waitingForWheelClose || _swapPauseHeld
                || IsWindowActive(_inventoryFullWindow) || IsWindowActive(_itemMaxLevelReachedWindow)
                || IsWindowActive(_rewardPopup) || _character == null)
                return;

            if (_pendingNewItemRewardIndex >= 0)
            {
                _wheelRewards[_pendingNewItemRewardIndex] = CreateNewItemReward(
                    _wheelRewards[_pendingNewItemRewardIndex]);
                _pendingNewItemRewardIndex = -1;
            }

            while (_nextRewardIndex < _wheelRewards.Count)
            {
                WheelReward reward = _wheelRewards[_nextRewardIndex];
                _nextRewardIndex++;

                switch (reward.Type)
                {
                    case WheelRewardType.Item:
                        _wheelRewards[_nextRewardIndex - 1] = GrantWheelItem(reward);
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

        private WheelReward GrantWheelItem(WheelReward reward)
        {
            Item existingItem = _character.Inventory.Items.FirstOrDefault(
                item => item.Data.ItemVariation == reward.Item.Variation);
            if (existingItem == null)
            {
                _character.SelectWheelItem(reward.Item.Variation);
                if (_waitingForItemWindow)
                {
                    _pendingNewItemRewardIndex = _nextRewardIndex - 1;
                    return reward;
                }

                return CreateNewItemReward(reward);
            }

            if (existingItem.IsMaxLevelReached())
            {
                _character.SelectWheelItem(reward.Item.Variation);
                return reward;
            }

            int previousLevel = existingItem.CurrentLevel;
            var previousStats = existingItem.UiStats
                .Where(stat => stat != null)
                .Select(stat => new Stat(stat))
                .ToList();

            _character.SelectWheelItem(reward.Item.Variation);

            return existingItem.CurrentLevel > previousLevel
                ? WheelReward.CreateItemUpgrade(existingItem, previousLevel, previousStats)
                : reward;
        }

        private WheelReward CreateNewItemReward(WheelReward reward)
        {
            Item newItem = _character.Inventory.Items.FirstOrDefault(
                item => item.Data.ItemVariation == reward.Item.Variation);
            return newItem != null ? WheelReward.CreateNewItem(newItem) : reward;
        }

        private void TryCompleteRewardBatch()
        {
            if (!_rewardPauseHeld || _swapPauseHeld || _waitingForWheelClose
                || IsWindowActive(_inventoryFullWindow) || IsWindowActive(_itemMaxLevelReachedWindow)
                || IsWindowActive(_rewardPopup)
                || (!_isGrantingRewards && IsWindowActive(_fortuneWheelWindow)))
                return;

            if (_wheelRewards.Count > 0 && (!_isGrantingRewards || _nextRewardIndex < _wheelRewards.Count))
                return;

            if (_wheelRewards.Count > 0)
            {
                _rewardPopup.ShowRewards(_wheelRewards, _wheelRewardBaseStats);
                _wheelRewards.Clear();
                return;
            }

            _wheelRewardBaseStats = null;
            _nextRewardIndex = 0;
            _pendingNewItemRewardIndex = -1;
            _isGrantingRewards = false;
            _waitingForItemWindow = false;
            _waitingForWheelClose = false;

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
            _character?.FinishPendingItemSwap();
            _waitingForItemWindow = false;

            if (_isGrantingRewards)
            {
                GrantNextRewards();
                return;
            }

            if (_rewardPauseHeld)
                TryCompleteRewardBatch();
            else if (_pendingLevelUps > 0)
            {
                HoldRewardPause();
                OpenPendingLevelUp();
            }
            else
                GameTimeScale.SetPauseActive(false);
        }

        private void OnInventoryFullWindowDeclined(int goldAmount)
        {
            if (_character == null || !_character.DeclinePendingItem(goldAmount))
                return;

            if (_pendingNewItemRewardIndex >= 0)
            {
                _wheelRewards[_pendingNewItemRewardIndex] = WheelReward.CreateGold(Mathf.Max(0, goldAmount));
                _pendingNewItemRewardIndex = -1;
            }
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
            if (_swapPauseHeld || _character == null || _character.PendingItemForSwap == null)
                return;

            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(false);

            _waitingForItemWindow = true;
            _swapPauseHeld = true;
            GameTimeScale.SetPauseActive(true);
            _inventoryFullWindow.Show(_character.PendingItemForSwap);
        }
    }
}
