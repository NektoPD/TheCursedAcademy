using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _name;
    
    public Slider Slider => _slider;

    public void SetName(string name) => _name.text = name;
}
