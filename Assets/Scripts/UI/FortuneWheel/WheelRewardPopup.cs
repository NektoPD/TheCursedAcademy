using System;
using System.Collections.Generic;
using System.Text;
using Data;
using Items.BaseClass;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG.LanguageLegacy;

namespace UI.FortuneWheel
{
    public class WheelRewardPopup : UI.Window
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Sprite _goldIcon;
        [SerializeField] private AudioClip _showClip;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private GameObject _rewardCardPrefab;

        public event Action Confirmed;

        private ScrollRect _rewardsScroll;
        private RectTransform _rewardsContent;
        private bool _confirmationPending;
        private TMP_Text _batchButtonLabel;
        private Vector2 _singleButtonSize;
        private Vector2 _singleButtonPosition;
        private Quaternion _singleButtonRotation;
        private readonly List<KeyValuePair<GameObject, bool>> _singleButtonVisuals =
            new List<KeyValuePair<GameObject, bool>>();

        private void OnEnable() => _closeButton.onClick.AddListener(OnClosePressed);

        private void OnDisable() => _closeButton.onClick.RemoveListener(OnClosePressed);

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

            // Snapshot before opening: callbacks may change the parent's mutable list.
            var displayRewards = new List<WheelReward>(rewards.Count);

            foreach (var reward in rewards)
            {
                if (IsDisplayable(reward))
                    displayRewards.Add(reward);
            }

            if (displayRewards.Count == 0)
                return;

            EnsureRewardsScroll();
            BeginShow(true);

            foreach (var reward in displayRewards)
            {
                AddRewardCard(reward);
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rewardsContent);
            _rewardsScroll.StopMovement();
            _rewardsContent.anchoredPosition = Vector2.zero;
            _rewardsScroll.horizontalNormalizedPosition = 0f;
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
            if (_rewardsScroll != null)
            {
                _rewardsScroll.StopMovement();
                _rewardsScroll.gameObject.SetActive(false);

                // Destroy is deferred; hide old rows immediately, including repeated Shows in one frame.
                for (int i = _rewardsContent.childCount - 1; i >= 0; i--)
                {
                    var row = _rewardsContent.GetChild(i).gameObject;
                    row.SetActive(false);
                    Destroy(row);
                }

                _rewardsContent.sizeDelta = Vector2.zero;
                _rewardsContent.anchoredPosition = Vector2.zero;
                _rewardsScroll.gameObject.SetActive(batch);
            }

            if (_icon != null)
                _icon.gameObject.SetActive(!batch);

            _title.gameObject.SetActive(!batch);
            _description.gameObject.SetActive(!batch);
            SetIcon(null);
            _title.text = string.Empty;
            _description.text = string.Empty;
            SetButtonMode(batch);
            _confirmationPending = true;
            _closeButton.interactable = true;

            OpenUnscaledTime();
            PlayShowSound();
        }

        private void SetButtonMode(bool batch)
        {
            var buttonRect = (RectTransform)_closeButton.transform;

            if (_batchButtonLabel == null)
            {
                if (!batch)
                    return;

                _singleButtonSize = buttonRect.sizeDelta;
                _singleButtonPosition = buttonRect.anchoredPosition;
                _singleButtonRotation = buttonRect.localRotation;

                foreach (Transform child in buttonRect)
                {
                    _singleButtonVisuals.Add(new KeyValuePair<GameObject, bool>(
                        child.gameObject, child.gameObject.activeSelf));
                }

                _batchButtonLabel = Instantiate(_title, buttonRect, false);
                DisablePlaceholderTranslations(_batchButtonLabel.gameObject);
                _batchButtonLabel.transform.localScale = Vector3.one;
                _batchButtonLabel.transform.localRotation = Quaternion.identity;
                _batchButtonLabel.raycastTarget = false;
                _batchButtonLabel.alignment = TextAlignmentOptions.Center;
                _batchButtonLabel.enableAutoSizing = true;
                _batchButtonLabel.fontSizeMin = 18f;
                _batchButtonLabel.fontSizeMax = _title.fontSize;
                _batchButtonLabel.margin = Vector4.zero;
                var labelRect = _batchButtonLabel.rectTransform;
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = new Vector2(12f, 8f);
                labelRect.offsetMax = new Vector2(-12f, -8f);
            }

            foreach (var visual in _singleButtonVisuals)
                visual.Key.SetActive(!batch && visual.Value);

            _batchButtonLabel.gameObject.SetActive(batch);
            _batchButtonLabel.text = batch
                ? Translator.Translate("Получить всё", "Claim all", "Hepsini al")
                : string.Empty;

            buttonRect.localRotation = batch ? Quaternion.identity : _singleButtonRotation;
            buttonRect.sizeDelta = batch ? new Vector2(260f, _singleButtonSize.y) : _singleButtonSize;
            buttonRect.anchoredPosition = batch
                ? new Vector2(-20f - 260f * (1f - buttonRect.pivot.x), _singleButtonPosition.y)
                : _singleButtonPosition;
        }

        private void EnsureRewardsScroll()
        {
            if (_rewardsScroll != null)
                return;

            // Use the existing description panel, not the popup root (only 100 x 100 in the scene).
            var viewport = CreateRect("RewardsScroll", _description.transform.parent);
            viewport.anchorMin = new Vector2(0.04f, 0.28f);
            viewport.anchorMax = new Vector2(0.96f, 0.96f);
            viewport.offsetMin = Vector2.zero;
            viewport.offsetMax = Vector2.zero;

            var surface = viewport.gameObject.AddComponent<Image>();
            surface.color = Color.clear;
            surface.raycastTarget = true;
            viewport.gameObject.AddComponent<RectMask2D>();

            _rewardsContent = CreateRect("Content", viewport);
            _rewardsContent.anchorMin = Vector2.zero;
            _rewardsContent.anchorMax = new Vector2(0f, 1f);
            _rewardsContent.pivot = new Vector2(0f, 1f);
            _rewardsContent.sizeDelta = Vector2.zero;

            var layout = _rewardsContent.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var fitter = _rewardsContent.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _rewardsScroll = viewport.gameObject.AddComponent<ScrollRect>();
            _rewardsScroll.viewport = viewport;
            _rewardsScroll.content = _rewardsContent;
            _rewardsScroll.horizontal = true;
            _rewardsScroll.vertical = false;
            _rewardsScroll.scrollSensitivity = 120f;
            _rewardsScroll.movementType = ScrollRect.MovementType.Clamped;
            _rewardsScroll.inertia = false;
            viewport.gameObject.SetActive(false);
        }

        private void AddRewardCard(WheelReward reward)
        {
            var card = Instantiate(_rewardCardPrefab, _rewardsContent, false);
            card.name = "Reward";

            DisablePlaceholderTranslations(card);

            string title = reward.Type == WheelRewardType.Gold
                ? Translator.Translate("Золото", "Gold", "Altın")
                : reward.Label;
            string description = reward.Type switch
            {
                WheelRewardType.Gold => "+" + reward.GoldAmount,
                WheelRewardType.Buff => GetBuffDescription(reward.Buff),
                _ => reward.Item.Description
            };
            Sprite sprite = reward.Type == WheelRewardType.Gold ? _goldIcon : reward.Sprite;
            card.GetComponentInChildren<ItemView>().ShowReward(title, description, sprite);
        }

        private static void DisablePlaceholderTranslations(GameObject visual)
        {
            // Dynamic labels must not be replaced by the prefab's placeholder translations.
            foreach (var translation in visual.GetComponentsInChildren<LanguageYG>(true))
                translation.enabled = false;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.gameObject.layer = parent.gameObject.layer;
            rect.SetParent(parent, false);
            return rect;
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

            return _audioSource;
        }

        private void OnClosePressed()
        {
            if (!_confirmationPending)
                return;

            _confirmationPending = false;
            _closeButton.interactable = false;
            Confirmed?.Invoke();
        }
    }
}
