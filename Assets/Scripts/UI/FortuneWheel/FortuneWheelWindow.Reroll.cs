using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG;

namespace UI.FortuneWheel
{
    public partial class FortuneWheelWindow
    {
        private const float RerollAdLoadTimeout = 15f;
        private static int _nextRerollRewardId = 1000;

        [Header("Rewarded Reroll")]
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TMP_Text _rerollButtonText;
        [SerializeField] private Button _claimButton;

        private bool _rerollUsed;
        private bool _waitingForRewardChoice;
        private bool _rerollRequested;
        private bool _rerollAdPending;
        private string _pendingRerollRewardId;
        private Coroutine _rerollAdLoadRoutine;

        private bool HasRewardChoiceControls => _claimButton != null;

        private void BindRerollControls()
        {
            if (_claimButton != null)
                _claimButton.onClick.AddListener(HandleClaimButton);

            if (_rerollButton != null)
                _rerollButton.onClick.AddListener(RequestRewardedReroll);

#if RewardedAdv_yg
            YG2.onOpenRewardedAdv += OnRerollAdOpened;
            YG2.onCloseRewardedAdv += FinishRerollAd;
            YG2.onErrorRewardedAdv += FinishRerollAd;
            YG2.onRewardAdv += OnRerollRewarded;
            YG2.onCloseAnyAdv += RefreshRewardChoiceControls;
            YG2.onErrorAnyAdv += RefreshRewardChoiceControls;
#endif
            ResetRewardChoice();
        }

        private void UnbindRerollControls()
        {
            if (_claimButton != null)
                _claimButton.onClick.RemoveListener(HandleClaimButton);

            if (_rerollButton != null)
                _rerollButton.onClick.RemoveListener(RequestRewardedReroll);

#if RewardedAdv_yg
            YG2.onOpenRewardedAdv -= OnRerollAdOpened;
            YG2.onCloseRewardedAdv -= FinishRerollAd;
            YG2.onErrorRewardedAdv -= FinishRerollAd;
            YG2.onRewardAdv -= OnRerollRewarded;
            YG2.onCloseAnyAdv -= RefreshRewardChoiceControls;
            YG2.onErrorAnyAdv -= RefreshRewardChoiceControls;
#endif
            ResetRewardChoice();
        }

        private void ResetRerollSession()
        {
            _rerollUsed = false;
            ResetRewardChoice();
        }

        private void ResetRewardChoice()
        {
            StopRerollAdLoadTimer();
            _waitingForRewardChoice = false;
            _rerollRequested = false;
            _rerollAdPending = false;
            _pendingRerollRewardId = null;
            SetStopButtonAvailable(false);

            RefreshRewardChoiceControls();
        }

        private void ShowRewardChoice()
        {
            _waitingForRewardChoice = true;
            RefreshRewardChoiceControls();
        }

        private void HandleClaimButton()
        {
            if (!_waitingForRewardChoice || _rerollAdPending)
                return;

            _waitingForRewardChoice = false;
            _pendingRerollRewardId = null;
            RefreshRewardChoiceControls();
        }

        private void RequestRewardedReroll()
        {
            if (!isActiveAndEnabled || _isDemo || !_waitingForRewardChoice || _rerollUsed
                || _rerollAdPending || !CanShowRerollAd())
                return;

#if RewardedAdv_yg
            _pendingRerollRewardId = (++_nextRerollRewardId).ToString(CultureInfo.InvariantCulture);
            _rerollAdPending = true;
            RefreshRewardChoiceControls();
            _rerollAdLoadRoutine = StartCoroutine(WaitForRerollAdToOpen(_pendingRerollRewardId));

            try
            {
                YG2.RewardedAdvShow(_pendingRerollRewardId);
            }
            catch (Exception exception)
            {
                FinishRerollAd();
                Debug.LogException(exception, this);
            }
#endif
        }

        private void OnRerollRewarded(string rewardId)
        {
            if (!isActiveAndEnabled || _isDemo || !_waitingForRewardChoice || _rerollUsed
                || string.IsNullOrEmpty(_pendingRerollRewardId) || rewardId != _pendingRerollRewardId)
                return;

            _rerollUsed = true;
            _rerollRequested = true;
            _waitingForRewardChoice = false;
            RefreshRewardChoiceControls();
        }

        private void OnRerollAdOpened()
        {
            if (_waitingForRewardChoice && !string.IsNullOrEmpty(_pendingRerollRewardId))
            {
                _rerollAdPending = true;
                StopRerollAdLoadTimer();
            }

            RefreshRewardChoiceControls();
        }

        private void FinishRerollAd()
        {
            StopRerollAdLoadTimer();
            _rerollAdPending = false;
            RefreshRewardChoiceControls();
        }

        private IEnumerator WaitForRerollAdToOpen(string rewardId)
        {
            yield return new WaitForSecondsRealtime(RerollAdLoadTimeout);
            _rerollAdLoadRoutine = null;

#if RewardedAdv_yg
            if (_rerollAdPending && _pendingRerollRewardId == rewardId && !YG2.nowRewardAdv)
                FinishRerollAd();
#endif
        }

        private void StopRerollAdLoadTimer()
        {
            if (_rerollAdLoadRoutine == null)
                return;

            StopCoroutine(_rerollAdLoadRoutine);
            _rerollAdLoadRoutine = null;
        }

        private static bool CanShowRerollAd()
        {
#if RewardedAdv_yg
            return !YG2.nowInterAdv && !YG2.nowRewardAdv;
#else
            return false;
#endif
        }

        private void RefreshRewardChoiceControls()
        {
            bool choosingReward = _waitingForRewardChoice && !_isDemo;
            if (_claimButton != null)
            {
                _claimButton.interactable = choosingReward && !_rerollAdPending;
                _claimButton.gameObject.SetActive(choosingReward);
            }

            if (_rerollButton == null)
                return;

            _rerollButton.gameObject.SetActive(choosingReward && !_rerollUsed);
            _rerollButton.interactable = choosingReward && !_rerollUsed && !_rerollAdPending
                && CanShowRerollAd();

            if (_rerollButtonText == null)
                return;

            _rerollButtonText.text = _rerollUsed
                ? Translator.Translate("Реролл\nиспользован", "Reroll\nused", "Yenileme\nkullanıldı")
                : _rerollAdPending
                    ? Translator.Translate("Загрузка\nрекламы", "Loading\nad", "Reklam\nyükleniyor")
                    : Translator.Translate("Реролл", "Reroll", "Yeniden çevir") + "\n<sprite=\"ad_icon\" index=0>";
        }
    }
}
