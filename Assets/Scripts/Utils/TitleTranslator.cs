using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Utils
{
    [RequireComponent(typeof(Image))]
    public class TitleTranslator : MonoBehaviour
    {
        [SerializeField] private Sprite _ruText;
        [SerializeField] private Sprite _engText;
        [SerializeField] private Sprite _trText;

        private Image _title;

        private void Awake()
        {
            _title = GetComponent<Image>();
        }

        private void Start()
        {
            string language = YandexGame.EnvironmentData.language;

            _title.sprite = language switch
            {
                "ru" => _ruText,
                "en" => _engText,
                "tr" => _trText,
                _ => _ruText,
            };
        }
    }
}