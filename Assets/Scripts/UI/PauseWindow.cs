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

    private void OnEnable()
    {
        _exit.onClick.AddListener(ChangeScene);
        YG2.onFocusWindowGame += OnFocusWindowGame;
    }

    private void OnDisable()
    {
        _exit.onClick.RemoveListener(ChangeScene);
        YG2.onFocusWindowGame -= OnFocusWindowGame;
        GameTimeScale.SetPauseActive(false);
        GameTimeScale.Set(1f);
        if (YG2.isFocusWindowGame)
            YG2.PauseGame(false);
    }

    private void Update()
    {
        if (!GameTimeScale.IsPauseActive)
            return;

        GameTimeScale.ForcePause();
        if (!YG2.isPauseGame)
            YG2.PauseGame(true);
    }

    public override void OpenWindow()
    {
        GameTimeScale.SetPauseActive(true);
        base.OpenWindow();
        YG2.PauseGame(true);
    }

    private void OnFocusWindowGame(bool isFocused)
    {
        if (isFocused && GameTimeScale.IsPauseActive)
            YG2.PauseGame(true);
    }

    private void ChangeScene()
    {
        CloseWindow();
        _changer.ChangeScene(_menuIdScene);
    }
}
