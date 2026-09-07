using System;
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

        private Canvas _hudCanvas;
        private bool _isTutorialMode;

        public event Action AbilityButtonPressed;

        private void Awake()
        {
            _hudCanvas = GetComponentInChildren<Canvas>(true);

            if (_abilityButton != null)
                _abilityButton.onClick.AddListener(() => AbilityButtonPressed?.Invoke());

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
                bool isAbilityButton = _abilityButton != null && child.gameObject == _abilityButton.gameObject;
                bool isAbilityText = _abilityDesktopPrompt != null && child.gameObject == _abilityDesktopPrompt;

                if (!isAbilityButton && !isAbilityText)
                    child.gameObject.SetActive(false);
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
            bool isMobile = !YandexGame.EnvironmentData.isDesktop;

            if (_abilityButton != null)
                _abilityButton.gameObject.SetActive(isMobile);

            if (_abilityDesktopPrompt != null)
                _abilityDesktopPrompt.SetActive(!isMobile);
        }

        public void HideAbilityUI()
        {
            if (_abilityButton != null)
                _abilityButton.gameObject.SetActive(false);

            if (_abilityDesktopPrompt != null)
                _abilityDesktopPrompt.SetActive(false);
        }
    }
}