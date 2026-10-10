using System;
using System.Collections.Generic;
using System.Text;
using Data;
using Items.BaseClass;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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

        public event Action Confirmed;

        private ScrollRect _rewardsScroll;
        private RectTransform _rewardsContent;
        private bool _confirmationPending;
        private TMP_Text _batchButtonLabel;
        private Vector2 _singleButtonSize;
        private Vector2 _singleButtonPosition;
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
            Canvas.ForceUpdateCanvases();

            float height = 0f;

            foreach (var reward in displayRewards)
            {
                AddRewardRow(reward, ref height);
            }

            _rewardsContent.sizeDelta = new Vector2(0f, height);
            _rewardsScroll.StopMovement();
            _rewardsContent.anchoredPosition = Vector2.zero;
            _rewardsScroll.verticalNormalizedPosition = 1f;
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

                foreach (Transform child in buttonRect)
                {
                    _singleButtonVisuals.Add(new KeyValuePair<GameObject, bool>(
                        child.gameObject, child.gameObject.activeSelf));
                }

                _batchButtonLabel = Instantiate(_title, buttonRect, false);
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

            float extraWidth = batch ? Mathf.Max(0f, 260f - _singleButtonSize.x) : 0f;
            buttonRect.sizeDelta = _singleButtonSize + new Vector2(extraWidth, 0f);
            // Keep the right edge fixed, leaving the list and the button in separate areas.
            buttonRect.anchoredPosition = _singleButtonPosition -
                new Vector2(extraWidth * (1f - buttonRect.pivot.x), 0f);
        }

        private void EnsureRewardsScroll()
        {
            if (_rewardsScroll != null)
                return;

            // Use the existing description panel, not the popup root (only 100 x 100 in the scene).
            var viewport = CreateRect("RewardsScroll", _description.transform.parent);
            viewport.anchorMin = new Vector2(0.04f, 0.04f);
            viewport.anchorMax = new Vector2(0.96f, 0.96f);
            viewport.offsetMin = Vector2.zero;
            viewport.offsetMax = Vector2.zero;

            var surface = viewport.gameObject.AddComponent<Image>();
            surface.color = Color.clear;
            surface.raycastTarget = true;
            viewport.gameObject.AddComponent<RectMask2D>();

            _rewardsContent = CreateRect("Content", viewport);
            _rewardsContent.anchorMin = new Vector2(0f, 1f);
            _rewardsContent.anchorMax = Vector2.one;
            _rewardsContent.pivot = new Vector2(0.5f, 1f);
            _rewardsContent.sizeDelta = Vector2.zero;

            _rewardsScroll = viewport.gameObject.AddComponent<ScrollRect>();
            _rewardsScroll.viewport = viewport;
            _rewardsScroll.content = _rewardsContent;
            _rewardsScroll.horizontal = false;
            _rewardsScroll.vertical = true;
            _rewardsScroll.scrollSensitivity = 120f;
            _rewardsScroll.movementType = ScrollRect.MovementType.Clamped;
            _rewardsScroll.inertia = false;
            viewport.gameObject.SetActive(false);
        }

        private void AddRewardRow(WheelReward reward, ref float contentHeight)
        {
            const float padding = 12f;
            const float iconSize = 72f;
            const float textLeft = iconSize + padding * 2f;
            const float spacing = 8f;

            var row = CreateRect("Reward", _rewardsContent);
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = Vector2.one;
            row.pivot = new Vector2(0.5f, 1f);
            row.anchoredPosition = new Vector2(0f, -contentHeight);

            if (_icon != null)
            {
                var icon = Instantiate(_icon, row, false);
                icon.gameObject.SetActive(true);
                icon.sprite = reward.Type == WheelRewardType.Gold ? _goldIcon : reward.Sprite;
                icon.enabled = icon.sprite != null;
                icon.preserveAspect = true;
                icon.raycastTarget = false;
                icon.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                icon.rectTransform.anchorMax = new Vector2(0f, 0.5f);
                icon.rectTransform.pivot = new Vector2(0f, 0.5f);
                icon.rectTransform.anchoredPosition = new Vector2(padding, 0f);
                icon.rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            }

            var label = Instantiate(_title, row, false);
            label.gameObject.SetActive(true);
            label.raycastTarget = false;
            label.enableAutoSizing = false;
            label.fontSize = _title.fontSize;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Overflow;
            label.margin = Vector4.zero;
            label.text = reward.Type switch
            {
                WheelRewardType.Gold => Translator.Translate("Золото", "Gold", "Altın") + "\n+" + reward.GoldAmount,
                WheelRewardType.Buff => reward.Buff.Name + "\n" + GetBuffDescription(reward.Buff),
                _ => reward.Item.Name
            };

            var textRect = label.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(textLeft, padding);
            textRect.offsetMax = new Vector2(-padding, -padding);

            float width = Mathf.Max(1f, _rewardsScroll.viewport.rect.width - textLeft - padding);
            float textHeight = label.GetPreferredValues(label.text, width, Mathf.Infinity).y;
            float rowHeight = Mathf.Max(iconSize, textHeight) + padding * 2f;
            row.sizeDelta = new Vector2(0f, rowHeight);
            contentHeight += rowHeight + spacing;
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
