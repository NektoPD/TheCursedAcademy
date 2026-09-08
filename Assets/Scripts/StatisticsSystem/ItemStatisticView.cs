using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace StatistiscSystem
{
    public class ItemStatisticView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _totalDamage;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _dps;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _totalDamageTitle;
        [SerializeField] private TextMeshProUGUI _levelTitle;
        [SerializeField] private TextMeshProUGUI _dpsTitle;
        [SerializeField] private TextMeshProUGUI _timeTitle;

        public void View(Sprite image, string totalDamage, string level, string dps, string time)
        {
            _totalDamageTitle.text = Translator.Translate("Общий урон", "Total damage", "Toplam hasar");
            _levelTitle.text = Translator.Translate("Уровень", "Level", "Seviye");
            _dpsTitle.text = Translator.Translate("Урон в секунду", "Damage per second", "Saniye başına hasar");
            _timeTitle.text = Translator.Translate("Время в кармане", "Time in inventory", "Envanterde geçirilen süre");

            _image.sprite = image;
            _totalDamage.text = totalDamage;
            _level.text = level;
            _dps.text = dps;
            _time.text = time;
        }
    }
}