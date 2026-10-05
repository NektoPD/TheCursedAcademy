using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG;

public class PauseWindow : Window
{
    [SerializeField] private Button _exit;
    [SerializeField] private int _menuIdScene;
    [SerializeField] private SceneChanger _changer;

    private bool _pauseHeld;

    private void OnEnable()
    {
        _exit.onClick.AddListener(ChangeScene);
    }

    private void OnDisable()
    {
        _exit.onClick.RemoveListener(ChangeScene);
        ReleasePause();
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        HoldPause();
    }

    private void HoldPause()
    {
        if (_pauseHeld)
            return;

        _pauseHeld = true;
        GamePauseController.HoldPause();
    }

    private void ReleasePause()
    {
        if (!_pauseHeld)
            return;

        _pauseHeld = false;
        GamePauseController.ReleasePause();
    }

    private void ChangeScene()
    {
        CloseWindow();
        _changer.ChangeScene(_menuIdScene);
    }
}
