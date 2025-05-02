using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatistiscSystem
{
    public class ItemStatisticView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _totalDamage;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _dps;
        [SerializeField] private TextMeshProUGUI _time;

        public void View(Sprite image, string totalDamage, string level, string dps, string time)
        {
            _image.sprite = image;
            _totalDamage.text = totalDamage;
            _level.text = level;
            _dps.text = dps;
            _time.text = time;
        }
    }
}