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
        [SerializeField] private GameObject _abilityDesktopPrompt;
        [SerializeField] private bool _forceMobileMode;

        private Canvas _hudCanvas;
        private bool _isTutorialMode;

        private Tween _abilityPulseTween;
        private Vector3 _abilityButtonInitialScale;

        public event Action AbilityButtonPressed;

        private void Awake()
        {
            _hudCanvas = GetComponentInChildren<Canvas>(true);

            if (_abilityButton != null)
            {
                _abilityButton.onClick.AddListener(() => AbilityButtonPressed?.Invoke());
                _abilityButtonInitialScale = _abilityButton.transform.localScale;
            }

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

                bool isAbilityText = _abilityDesktopPrompt != null &&
                                     child.gameObject == _abilityDesktopPrompt;

                bool containsAbilityBar = _abilityLevel != null &&
                                          _abilityLevel.transform.IsChildOf(child);

                if (!isAbilityButton && !isAbilityText)
                    child.gameObject.SetActive(false);
            }
        }

        private void HideNonAbilityChildren(Transform parent, Transform abilityBar)
        {
            foreach (Transform child in parent)
            {
                bool containsAbilityBar = child == abilityBar || abilityBar.IsChildOf(child);
                child.gameObject.SetActive(containsAbilityBar);

                if (containsAbilityBar && child != abilityBar)
                    HideNonAbilityChildren(child, abilityBar);
            }
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

        public void SetHeroImage(Sprite image)
        {
            _heroImage.sprite = image;
        }

        public void ShowAbilityReady()
        {
            bool isMobile;

#if UNITY_EDITOR
            isMobile = _forceMobileMode;
#else
            isMobile = !YandexGame.EnvironmentData.isDesktop;
#endif

            if (_abilityButton != null)
            {
                _abilityButton.gameObject.SetActive(isMobile);

                if (isMobile)
                    StartAbilityPulse();
                else
                    StopAbilityPulse();
            }

            if (_abilityDesktopPrompt != null)
                _abilityDesktopPrompt.SetActive(!isMobile);
        }

        public void HideAbilityUI()
        {
            if (_abilityButton != null)
            {
                _abilityButton.gameObject.SetActive(false);
                StopAbilityPulse();
            }

            if (_abilityDesktopPrompt != null)
                _abilityDesktopPrompt.SetActive(false);
        }

        private void StartAbilityPulse()
        {
            if (_abilityButton == null)
                return;

            if (_abilityPulseTween == null)
            {
                _abilityPulseTween = _abilityButton.transform
                    .DOScale(_abilityButtonInitialScale * 1.1f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetAutoKill(false);
            }

            _abilityPulseTween.Play();
        }

        private void StopAbilityPulse()
        {
            if (_abilityPulseTween == null)
                return;

            _abilityPulseTween.Pause();
            _abilityButton.transform.localScale = _abilityButtonInitialScale;
        }

        private void OnDestroy()
        {
            _abilityPulseTween?.Kill();
        }
    }
}