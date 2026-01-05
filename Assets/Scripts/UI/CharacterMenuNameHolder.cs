using System;
using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CharacterMenuNameHolder : MonoBehaviour
    {
        [SerializeField] private CharacterVisualData _characterVisualData;
        [SerializeField] private TextMeshProUGUI _name;

        private void OnEnable()
        {
            _name.text = _characterVisualData.Name;
        }
    }
}