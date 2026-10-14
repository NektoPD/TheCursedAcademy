using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.FortuneWheel
{
    public partial class WheelRewardPopup
    {
        [Header("Reward Appearance")]
        [SerializeField, Min(0.01f)] private float _rewardAppearDuration = 0.3f;
        [SerializeField, Min(0f)] private float _rewardAppearDelay = 0.08f;
        [SerializeField] private AudioClip _rewardAppearClip;

        [Header("Reward Scrolling")]
        [SerializeField] private GameObject _scrollIndicator;
        [SerializeField, Min(0.01f)] private float _autoScrollDuration = 0.45f;

        private Coroutine _presentationRoutine;
        private Tween _scrollTween;
        private bool _isPopupOpen;
        private bool _isShowingRewards;

        private void OnPopupOpened()
        {
            _isPopupOpen = true;
        }

        private void LateUpdate()
        {
            UpdateScrollIndicator();
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
                    _ => reward.Item.Description
                };
                Sprite sprite = reward.Type == WheelRewardType.Gold ? _goldIcon : reward.Sprite;
                card.Initialize(title, description, sprite);
                card.PrepareAppearance();
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
            UpdateScrollIndicator();
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

        private void UpdateScrollIndicator()
        {
            GameObject indicator = _scrollIndicator;
            if (indicator == null && _rewardsScroll != null)
            {
                var scrollbar = _rewardsScroll.horizontal
                    ? _rewardsScroll.horizontalScrollbar
                    : _rewardsScroll.verticalScrollbar;
                if (scrollbar != null)
                    indicator = scrollbar.gameObject;
            }

            if (indicator == null)
                return;

            bool visible = _isShowingRewards && HasScrollOverflow();
            if (indicator.activeSelf != visible)
                indicator.SetActive(visible);
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

            UpdateScrollIndicator();
        }
    }
}
