using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class PauseWindow : Window
{
    [SerializeField] private Button _open;
    [SerializeField] private Button _close;
    [SerializeField] private Button _exit;
    [SerializeField] private SceneAsset _menu;
    [SerializeField] private SceneChanger _changer;

    private void OnEnable()
    {
        _open.onClick.AddListener(Show);
        _close.onClick.AddListener(Close);
        _exit.onClick.AddListener(ChangeScene);
    }

    private void OnDisable()
    {
        _open.onClick.RemoveListener(Show);
        _close.onClick.RemoveListener(Close);
        _exit.onClick.RemoveListener(ChangeScene);
    }

    private void ChangeScene()
    {
        Close();
        _changer.ChangeScene(_menu);
    }
}
