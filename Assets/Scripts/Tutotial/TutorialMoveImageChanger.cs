using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
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

        private void OnEnable()
        {
#if !UNITY_EDITOR
            YG2.onGetSDKData += RefreshHint;
#endif
            RefreshHint();
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            YG2.onGetSDKData -= RefreshHint;
#endif
        }

        private void RefreshHint()
        {
            bool useTouch = TouchControls.IsEnabled(_forceMobileMode);
            _image.sprite = useTouch ? _joystick : _wasd;
            _joystickText.gameObject.SetActive(useTouch);
            _keyboardText.gameObject.SetActive(!useTouch);
        }
    }
}