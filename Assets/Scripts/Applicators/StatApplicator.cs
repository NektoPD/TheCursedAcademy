using TMPro;
using UnityEngine;
using YG;

public class StatApplicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private LanguageYG _nameYG;

    public void Applicate(string name, float value)
    {
        _name.text = name;
        _value.text = value.ToString();
    }
}
