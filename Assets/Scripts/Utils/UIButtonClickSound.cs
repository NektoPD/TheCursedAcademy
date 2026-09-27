using Data;
using UI.Applicators.ClickHandlers;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private Button[] _buttons;
    private PerkClickHandler[] _perkClickHandlers;
    private CharacterClickHandler[] _characterClickHandlers;

    private void Awake()
    {
        if (_audioSource == null)
        {
            Debug.LogError($"{nameof(UIButtonClickSound)}: AudioSource не назначен");
            return;
        }

        _buttons = FindObjectsOfType<Button>(true);
        _perkClickHandlers = FindObjectsOfType<PerkClickHandler>(true);
        _characterClickHandlers = FindObjectsOfType<CharacterClickHandler>(true);
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

        if (_perkClickHandlers != null)
            foreach (var handler in _perkClickHandlers)
                handler.Clicked += OnPerkClicked;

        if (_characterClickHandlers != null)
            foreach (var handler in _characterClickHandlers)
                handler.Clicked += OnCharacterClicked;
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

        if (_perkClickHandlers != null)
            foreach (var handler in _perkClickHandlers)
                handler.Clicked -= OnPerkClicked;

        if (_characterClickHandlers != null)
            foreach (var handler in _characterClickHandlers)
                handler.Clicked -= OnCharacterClicked;
    }

    private void OnPerkClicked(PerkVisualData data)
    {
        PlayClickSound();
    }

    private void OnCharacterClicked(CharacterVisualData data)
    {
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (_audioSource == null)
            return;

        _audioSource.Play();
    }
}
