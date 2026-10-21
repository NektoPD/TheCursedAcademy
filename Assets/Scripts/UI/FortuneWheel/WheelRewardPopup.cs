using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Data;
using DG.Tweening;
using Items.BaseClass;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI.FortuneWheel
{
    public class WheelRewardPopup : UI.Window
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
        [SerializeField] private WheelRewardStatsView _statsView;

        [Header("Reward Appearance")]
        [SerializeField, Min(0.01f)] private float _rewardAppearDuration = 0.3f;
        [SerializeField, Min(0f)] private float _rewardAppearDelay = 0.08f;
        [SerializeField] private AudioClip _rewardAppearClip;

        [Header("Reward Scrolling")]
        [SerializeField, Min(0.01f)] private float _autoScrollDuration = 0.45f;

        private readonly List<WheelRewardView> _spawnedRewards = new();
        private bool _confirmationPending;
        private Coroutine _presentationRoutine;
        private Tween _scrollTween;
        private bool _isPopupOpen;
        private bool _isShowingRewards;

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

        private void OnPopupOpened()
        {
            _isPopupOpen = true;
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

        public void ShowRewards(IReadOnlyList<WheelReward> rewards,
            IReadOnlyDictionary<PerkType, float> baseStats = null)
        {
            if (rewards == null || rewards.Count == 0)
                return;

            if (_claimButton == null || _rewardsContent == null || _rewardCardPrefab == null)
            {
                return;
            }

            var displayRewards = new List<WheelReward>(rewards.Count);
            foreach (var reward in rewards)
            {
                if (IsDisplayable(reward))
                    displayRewards.Add(reward);
            }

            if (displayRewards.Count == 0)
                return;

            BeginShow(true, baseStats);
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

        private void BeginShow(bool batch, IReadOnlyDictionary<PerkType, float> baseStats = null)
        {
            StopPresentation();
            if (_statsView != null)
                _statsView.BeginPreview(baseStats);

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

        private IEnumerator RevealRewards(IReadOnlyList<WheelReward> rewards)
        {
            while (!_isPopupOpen)
                yield return null;

            for (int i = 0; i < rewards.Count; i++)
            {
                WheelReward reward = rewards[i];
                var card = Instantiate(_rewardCardPrefab, _rewardsContent, false);
                _spawnedRewards.Add(card);
                card.gameObject.SetActive(true);

                string title = reward.Type == WheelRewardType.Gold
                    ? Translator.Translate("Золото", "Gold", "Altın")
                    : reward.Label;
                string description = reward.Type switch
                {
                    WheelRewardType.Gold => "+" + reward.GoldAmount,
                    WheelRewardType.Buff => GetBuffDescription(reward.Buff),
                    _ => reward.ItemUpgradeDescription ?? reward.Item.Description
                };
                Sprite sprite = reward.Type == WheelRewardType.Gold ? _goldIcon : reward.Sprite;
                card.Initialize(title, description, sprite);
                card.PrepareAppearance();
                if (reward.Type == WheelRewardType.Buff && _statsView != null)
                    _statsView.AddBuff(reward.Buff);
                RebuildRewardLayout();

                if (i == 0 && _rewardsScroll != null)
                {
                    _rewardsScroll.StopMovement();
                    _rewardsScroll.horizontalNormalizedPosition = 0f;
                    _rewardsScroll.verticalNormalizedPosition = 1f;
                }

                AudioClip clip = _rewardAppearClip != null ? _rewardAppearClip : _showClip;
                if (clip != null)
                    GetAudioSource().PlayOneShot(clip);

                yield return card.PlayAppearance(_rewardAppearDuration).WaitForCompletion();

                if (i < rewards.Count - 1 && _rewardAppearDelay > 0f)
                    yield return new WaitForSecondsRealtime(_rewardAppearDelay);
            }

            RebuildRewardLayout();
            if (rewards.Count > 3 && HasScrollOverflow())
            {
                _rewardsScroll.StopMovement();
                Vector2 start = _rewardsScroll.normalizedPosition;
                Vector2 end = new Vector2(
                    _rewardsScroll.horizontal ? 1f : start.x,
                    _rewardsScroll.vertical ? 0f : start.y);
                _scrollTween = DOTween.To(
                        () => _rewardsScroll.normalizedPosition,
                        position => _rewardsScroll.normalizedPosition = position,
                        end,
                        Mathf.Max(0.01f, _autoScrollDuration))
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
                yield return _scrollTween.WaitForCompletion();
                _scrollTween = null;
            }

            _presentationRoutine = null;
            _claimButton.interactable = true;
        }

        private void RebuildRewardLayout()
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rewardsContent);
        }

        private bool HasScrollOverflow()
        {
            if (_rewardsScroll == null || _rewardsContent == null)
                return false;

            RectTransform viewport = _rewardsScroll.viewport != null
                ? _rewardsScroll.viewport
                : (RectTransform)_rewardsScroll.transform;
            return (_rewardsScroll.horizontal && _rewardsContent.rect.width > viewport.rect.width + 1f)
                || (_rewardsScroll.vertical && _rewardsContent.rect.height > viewport.rect.height + 1f);
        }

        private void StopPresentation()
        {
            if (_presentationRoutine != null)
                StopCoroutine(_presentationRoutine);
            _presentationRoutine = null;
            _scrollTween?.Kill();
            _scrollTween = null;
            _isShowingRewards = false;
            _isPopupOpen = false;
            _confirmationPending = false;
            if (_claimButton != null)
                _claimButton.interactable = false;

            foreach (var card in _spawnedRewards)
            {
                if (card != null)
                    card.StopAppearance();
            }

            if (_statsView != null)
                _statsView.StopAppearance();
        }
    }
}
