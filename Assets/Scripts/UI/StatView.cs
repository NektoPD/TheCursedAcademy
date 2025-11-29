using TMPro;
using UnityEngine;
using YG;
using YG.LanguageLegacy;

namespace UI
{
    public class StatView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private LanguageYG _nameYG;

        public void Applicate(string name, string value)
        {
            _name.text = name;

            _value.text = value;
        }
    }
}