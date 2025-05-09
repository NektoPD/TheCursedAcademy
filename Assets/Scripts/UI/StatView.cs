using TMPro;
using UnityEngine;
using YG;

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

            if (value == "1")
                value = "0";

            _value.text = value;
        }
    }
}