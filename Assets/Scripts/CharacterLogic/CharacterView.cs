using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CharacterLogic
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Slider _levelBar;
        [SerializeField] private Image _heroImage;
        [SerializeField] private Image _abilityLevel;
        [SerializeField] private Button _abilityButton;
        [SerializeField] private Image _abilityDesktopElement;
        [SerializeField] private bool _forceMobileMode;

        private Canvas _hudCanvas;
        private bool _isTutorialMode;

        private Tween _abilityPulseTween;
        private Tween _levelPulseTween;
        private Tween _abilityDesktopTween;
        private Vector3 _abilityButtonInitialScale;
        private Vector3 _abilityDesktopImageInitialScale;
        private Vector3 _levelBarInitialScale;
        private bool _isMobile;

        public event Action AbilityButtonPressed;

        private void Awake()
        {
            _hudCanvas = GetComponentInChildren<Canvas>(true);
            if (_levelBar != null)
                _levelBarInitialScale = _levelBar.transform.localScale;

            if (_abilityButton != null)
            {
                _abilityButton.onClick.AddListener(() => AbilityButtonPressed?.Invoke());
                _abilityButtonInitialScale = _abilityButton.transform.localScale;
            }

            if (_abilityDesktopElement != null)
            {
                _abilityDesktopImageInitialScale = _abilityDesktopElement.transform.localScale;
            }

            SetupMobileMode();
            HideAbilityUI();
        }

        public void SetHudVisible(bool isVisible)
        {
            CacheHudCanvas();

            if (_hudCanvas != null)
                _hudCanvas.gameObject.SetActive(_isTutorialMode || isVisible);
        }

        public void SetTutorialMode(bool isTutorial)
        {
            _isTutorialMode = isTutorial;

            if (!isTutorial)
                return;

            CacheHudCanvas();

            if (_hudCanvas == null)
                return;

            _hudCanvas.gameObject.SetActive(true);

            foreach (Transform child in _hudCanvas.transform)
            {
                bool isAbilityButton = _abilityButton != null &&
                                       child.gameObject == _abilityButton.gameObject;

                bool isAbilityText = _abilityDesktopElement != null &&
                                     child.gameObject == _abilityDesktopElement;

                bool containsAbilityBar = _abilityLevel != null &&
                                          _abilityLevel.transform.IsChildOf(child);

                if (!isAbilityButton && !isAbilityText)
                    child.gameObject.SetActive(false);
            }
        }

        private void SetupMobileMode()
        {
#if UNITY_EDITOR
            _isMobile = _forceMobileMode;
#else
            _isMobile = !YandexGame.EnvironmentData.isDesktop;
#endif
        }

        private void CacheHudCanvas()
        {
            if (_hudCanvas == null)
                _hudCanvas = GetComponentInChildren<Canvas>(true);
        }

        public void UpdateHpBar(float value, float maxHealth)
        {
            _hpBar.value = value / maxHealth;
        }

        public void UpdateAbilityLevelBar(float value, float maxValue)
        {
            _abilityLevel.fillAmount = value / maxValue;
        }

        public void UpdateLevelBar(int value, int levelRequirement)
        {
            _levelBar.value = (float)value / levelRequirement;
        }

        public void PlayLevelUpPulse()
        {
            if (_levelBar == null)
                return;

            _levelPulseTween?.Kill();
            _levelBar.transform.localScale = _levelBarInitialScale;
            _levelPulseTween = _levelBar.transform
                .DOScale(_levelBarInitialScale * 1.15f, 0.18f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void SetHeroImage(Sprite image)
        {
            _heroImage.sprite = image;
        }

        public void ShowAbilityReady()
        {
            _abilityButton.gameObject.SetActive(_isMobile);
            _abilityDesktopElement.gameObject.SetActive(!_isMobile);

            if (_isMobile)
                PlayPulseAnimation(_abilityPulseTween, _abilityButton.transform, _abilityButtonInitialScale);
            else
                PlayPulseAnimation(_abilityDesktopTween, _abilityDesktopElement.transform,
                    _abilityDesktopImageInitialScale);
        }

        public void HideAbilityUI()
        {
            if (_isMobile)
            {
                _abilityButton.gameObject.SetActive(false);
                StopPlayingAnimation(_abilityPulseTween, _abilityButton.transform, _abilityButtonInitialScale);
                return;
            }

            _abilityDesktopElement.gameObject.SetActive(false);
            StopPlayingAnimation(_abilityDesktopTween, _abilityDesktopElement.transform,
                _abilityDesktopImageInitialScale);
        }

        private void PlayPulseAnimation(Tween tween, Transform transform, Vector3 initalScale)
        {
            tween ??= transform
                .DOScale(initalScale * 1.1f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);

            tween.Play();
        }

        private void StopPlayingAnimation(Tween tween, Transform transform, Vector3 initalScale)
        {
            if (tween == null)
                return;

            tween.Pause();
            transform.transform.localScale = initalScale;
        }

        private void OnDestroy()
        {
            _abilityPulseTween?.Kill();
            _levelPulseTween?.Kill();
        }
    }
}