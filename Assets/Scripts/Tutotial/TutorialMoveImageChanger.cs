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

        [SerializeField] private bool _forceMobileMode;

        private void Start()
        {
            bool  isDesktop = true;
#if UNITY_EDITOR
             isDesktop = !_forceMobileMode;
#else
isDesktop = YandexGame.EnvironmentData.isDesktop;
#endif
            if (isDesktop)
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