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

        public event Action AbilityButtonPressed;

        private void Awake()
        {
            if (_abilityButton != null)
                _abilityButton.onClick.AddListener(() => AbilityButtonPressed?.Invoke());

            HideAbilityUI();
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