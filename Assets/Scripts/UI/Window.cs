using UnityEngine;

namespace UI
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private GameObject _window;

        public void Show()
        {
            Time.timeScale = 0;
            _window.SetActive(true);
        }

        public void Close()
        {
            _window.SetActive(false);
            Time.timeScale = 1;
        }
    }
}