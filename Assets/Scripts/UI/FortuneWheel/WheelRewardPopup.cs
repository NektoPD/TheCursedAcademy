using System;
using System.Collections.Generic;
using System.Text;
using Data;
using Items.BaseClass;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI.FortuneWheel
{
    public partial class WheelRewardPopup : UI.Window
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [FormerlySerializedAs("_closeButton")]
        [SerializeField] private Button _claimButton;
        [SerializeField] private Sprite _goldIcon;
        [SerializeField] private AudioClip _showClip;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private WheelRewardView _rewardCardPrefab;
        [SerializeField] private RectTransform _rewardsContent;
        [SerializeField] private ScrollRect _rewardsScroll;

        private readonly List<WheelRewardView> _spawnedRewards = new();
        private bool _confirmationPending;

        public event Action Confirmed;

        private void OnEnable()
        {
            Opened += OnPopupOpened;
            if (_claimButton != null)
                _claimButton.onClick.AddListener(OnClaimPressed);
        }

        private void OnDisable()
        {
            Opened -= OnPopupOpened;
            StopPresentation();
            if (_claimButton != null)
                _claimButton.onClick.RemoveListener(OnClaimPressed);
        }

        public override void CloseWindow()
        {
            StopPresentation();
            base.CloseWindow();
        }

        public override void CloseUnscaledTime()
        {
            StopPresentation();
            base.CloseUnscaledTime();
        }

        public void ShowItem(ItemVisualData item)
        {
            ShowItem(item, null);
        }

        public void ShowItem(ItemVisualData item, Item existingItem)
        {
            if (item == null)
                return;

            BeginShow(false);
            SetIcon(item.Sprite);
            _title.text = item.Name;

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(item.Description))
                builder.AppendLine(item.Description);

            var stats = existingItem != null ? existingItem.UiStats : item.Stats;

            if (stats != null)
            {
                foreach (var stat in stats)
                {
                    if (stat == null)
                        continue;

                    string value = existingItem != null && !existingItem.IsMaxLevelReached()
                        ? stat.CurrentValue.ToString("0.##") + " -> " + stat.NextValue.ToString("0.##")
                        : stat.CurrentValue.ToString("0.##");
                    builder.AppendLine(stat.Name + ": " + value);
                }
            }

            _description.text = builder.ToString();
        }

        public void ShowGold(int amount)
        {
            BeginShow(false);
            SetIcon(_goldIcon);
            _title.text = Translator.Translate("Золото", "Gold", "Altın");
            _description.text = "+" + amount;
        }

        public void ShowBuff(WheelBuffData buff)
        {
            if (buff == null)
                return;

            BeginShow(false);
            SetIcon(buff.Icon);
            _title.text = buff.Name;
            _description.text = GetBuffDescription(buff);
        }

        public void ShowRewards(IReadOnlyList<WheelReward> rewards)
        {
            if (rewards == null || rewards.Count == 0)
                return;

            if (_claimButton == null || _rewardsContent == null || _rewardCardPrefab == null)
            {
                Debug.LogError("WheelRewardPopup: assign Claim Button, Rewards Content and Reward Card Prefab.", this);
                return;
            }

            // Snapshot before opening: callbacks may change the parent's mutable list.
            var displayRewards = new List<WheelReward>(rewards.Count);
            foreach (var reward in rewards)
            {
                if (IsDisplayable(reward))
                    displayRewards.Add(reward);
            }

            if (displayRewards.Count == 0)
                return;

            BeginShow(true);
            _presentationRoutine = StartCoroutine(RevealRewards(displayRewards));
        }

        private static bool IsDisplayable(WheelReward reward)
        {
            if (reward == null)
                return false;

            return reward.Type switch
            {
                WheelRewardType.Item => reward.Item != null,
                WheelRewardType.Gold => true,
                WheelRewardType.Buff => reward.Buff != null,
                _ => false
            };
        }

        private static string GetBuffDescription(WheelBuffData buff)
        {
            return Translator.Translate(
                "x" + buff.Multiplier.ToString("0.##") + " на " + buff.DurationSeconds.ToString("0.##") + " сек",
                "x" + buff.Multiplier.ToString("0.##") + " for " + buff.DurationSeconds.ToString("0.##") + " sec",
                "x" + buff.Multiplier.ToString("0.##") + " " + buff.DurationSeconds.ToString("0.##") + " sn");
        }

        private void BeginShow(bool batch)
        {
            StopPresentation();
            _isShowingRewards = batch;
            _isPopupOpen = false;
            foreach (var card in _spawnedRewards)
            {
                if (card == null)
                    continue;

                card.gameObject.SetActive(false);
                Destroy(card.gameObject);
            }
            _spawnedRewards.Clear();

            if (_rewardsScroll != null)
            {
                _rewardsScroll.StopMovement();
                _rewardsScroll.gameObject.SetActive(batch);
            }
            if (_rewardsContent != null)
                _rewardsContent.gameObject.SetActive(batch);

            if (_icon != null)
                _icon.gameObject.SetActive(!batch);
            if (_title != null)
            {
                _title.gameObject.SetActive(!batch);
                _title.text = string.Empty;
            }
            if (_description != null)
            {
                _description.gameObject.SetActive(!batch);
                _description.text = string.Empty;
            }

            SetIcon(null);
            _confirmationPending = true;
            _claimButton.interactable = !batch;
            OpenUnscaledTime();
            PlayShowSound();
        }

        private void SetIcon(Sprite sprite)
        {
            if (_icon == null)
                return;

            _icon.sprite = sprite;
            _icon.enabled = sprite != null;
        }

        private void PlayShowSound()
        {
            if (_showClip != null)
                GetAudioSource().PlayOneShot(_showClip);
        }

        private AudioSource GetAudioSource()
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f;
            }

            _audioSource.ignoreListenerPause = true;
            return _audioSource;
        }

        private void OnClaimPressed()
        {
            if (!_confirmationPending || _presentationRoutine != null)
                return;

            _confirmationPending = false;
            _claimButton.interactable = false;
            Confirmed?.Invoke();
        }
    }
}
