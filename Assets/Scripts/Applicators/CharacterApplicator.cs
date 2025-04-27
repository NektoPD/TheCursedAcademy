using CharacterLogic.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Applicators
{
    public class CharacterApplicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _item;
        [SerializeField] private CharacterClickHandler[] _characters;
        [SerializeField] private CharacterData _defaultCharacter;

        private void Start()
        {
            Application(_defaultCharacter);
        }

        private void OnEnable()
        {
            foreach (var character in _characters)
                character.Clicked += OnClick;
        }

        private void OnDisable()
        {
            foreach (var character in _characters)
                character.Clicked -= OnClick;
        }

        private void OnClick(CharacterData data) => Application(data);

        private void Application(CharacterData data)
        {
            _name.text = data.Type.ToString();
            _description.text = data.Type.ToString();
            _item.sprite = data.StartItem.Data.ItemIcon;
        }
    }
}