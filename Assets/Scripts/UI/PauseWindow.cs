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
    }

    private void OnFocusWindowGame(bool isFocused)
    {
        if (isFocused)
            Time.timeScale = 0f;
    }

    private void ChangeScene()
    {
        CloseWindow();
        _changer.ChangeScene(_menuIdScene);
    }
}