using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FortuneWheel
{
    public class WheelRewardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;

        public void Initialize(string title, string description, Sprite icon)
        {
            _title.text = title;
            _description.text = description;
            _icon.sprite = icon;
            _icon.enabled = icon != null;
        }
    }
}
