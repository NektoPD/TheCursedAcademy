using CharacterLogic;
using CharacterLogic.Initializer;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RoundCoinsView : MonoBehaviour
    {
        [SerializeField] private CharacterInitializer _characterInitializer;
        [SerializeField] private TextMeshProUGUI _text;

        private Character _character;

        private void OnEnable()
        {
            _characterInitializer.CharacterCreated += OnCharacterCreated;
        }

        private void OnDisable()
        {
            if (_characterInitializer != null)
                _characterInitializer.CharacterCreated -= OnCharacterCreated;

            UnbindCharacter();
        }

        private void OnCharacterCreated(Character character)
        {
            UnbindCharacter();
            _character = character;
            _character.RoundCoinsChanged += UpdateCoins;
            UpdateCoins(0);
        }

        private void UnbindCharacter()
        {
            if (_character == null)
                return;

            _character.RoundCoinsChanged -= UpdateCoins;
            _character = null;
        }

        private void UpdateCoins(int coins)
        {
            _text.text = coins.ToString();
        }
    }
}
