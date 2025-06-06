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

        private void Start()
        {
            if(YandexGame.EnvironmentData.isDesktop)
                _image.sprite = _wasd;
            else
                _image.sprite = _joystick;
        }
    }
}
