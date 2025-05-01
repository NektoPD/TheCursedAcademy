using UnityEngine;

namespace UI
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private GameObject _window;

        public void Show()
        {
            _window.SetActive(true);
            Time.timeScale = 0;
        }

        public void Close()
        {
            Time.timeScale = 1;
            _window.SetActive(false);
        }
    }
}