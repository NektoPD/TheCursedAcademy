using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FortuneWheel
{
    public class WheelSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;

        public void Set(WheelReward reward)
        {
            if (reward == null)
                return;

            if (_icon != null)
            {
                Sprite sprite = reward.Sprite;
                _icon.sprite = sprite;
                _icon.enabled = sprite != null;
            }

            if (_label != null)
                _label.text = reward.Label;
        }
    }
}
