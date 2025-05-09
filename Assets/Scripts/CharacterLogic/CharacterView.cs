using System;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterLogic
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Slider _levelBar;
        [SerializeField] private Image _heroImage;

        public void UpdateHpBar(float value, float maxHealth)
        {
            _hpBar.value = value / maxHealth;
        }

        public void UpdateLevelBar(int value, int levelRequirement)
        {
            _levelBar.value = (float)value / levelRequirement;
        }

        public void SetHeroImage(Sprite image)
        {
            _heroImage.sprite = image;
        }
    }
}