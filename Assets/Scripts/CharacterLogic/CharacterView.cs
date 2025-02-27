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

        public void UpdateLevelBar(float value, float levelRequieremnt)
        {
            _levelBar.value = value / levelRequieremnt;
        }

        public void SetHeroImage(Sprite image)
        {
            _heroImage.sprite = image;
        }
    }
}
