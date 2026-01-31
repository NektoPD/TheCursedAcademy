using UnityEngine;
using UnityEngine.UI;

public class UIButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private Button[] _buttons;

    private void Awake()
    {
        if (_audioSource == null)
        {
            Debug.LogError($"{nameof(UIButtonClickSound)}: AudioSource не назначен");
            return;
        }

        _buttons = FindObjectsOfType<Button>(true);
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_buttons == null)
            return;

        foreach (var button in _buttons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveListener(PlayClickSound);
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void Unsubscribe()
    {
        if (_buttons == null)
            return;

        foreach (var button in _buttons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (_audioSource == null)
            return;

        _audioSource.Play();
    }
}