using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Tutorial
{
    public class TutorialMoveImageChanger : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _wasd;
        [SerializeField] private Sprite _joystick;
        [SerializeField] private TMP_Text _joystickText;
        [SerializeField] private TMP_Text _keyboardText;

        private void Start()
        {
            if (YandexGame.EnvironmentData.isDesktop)
            {
                _image.sprite = _wasd;
                _joystickText.gameObject.SetActive(false);
                _keyboardText.gameObject.SetActive(true);
            }
            else
            {
                _image.sprite = _joystick;
                _joystickText.gameObject.SetActive(true);
                _keyboardText.gameObject.SetActive(false);
            }
        }
    }
}
