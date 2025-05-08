using UnityEngine;

namespace UI
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private GameObject _window;

        public virtual void OpenWindow() => _window.SetActive(true);

        public void CloseWindow() => _window.SetActive(false);

        private void StopTime() => Time.timeScale = 0f;

        private void StartTime() => Time.timeScale = 1f;
    }
}