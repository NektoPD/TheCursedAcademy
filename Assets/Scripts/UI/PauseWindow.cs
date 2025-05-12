using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class PauseWindow : Window
{
    [SerializeField] private Button _exit;
    [SerializeField] private int _menuIdScene;
    [SerializeField] private SceneChanger _changer;

    private void OnEnable()
    {
        _exit.onClick.AddListener(ChangeScene);
    }

    private void OnDisable()
    {
        _exit.onClick.RemoveListener(ChangeScene);
    }

    private void ChangeScene()
    {
        CloseWindow();
        _changer.ChangeScene(_menuIdScene);
    }
}
