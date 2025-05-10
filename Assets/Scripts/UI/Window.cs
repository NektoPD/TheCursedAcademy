using UI.Animation;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(WindowAnimation))]
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private GameObject _window;

        private WindowAnimation _windowAnimation;

        private void Awake()
        {
            _windowAnimation = GetComponent<WindowAnimation>();
        }

        public virtual void OpenWindow()
        {
            gameObject.SetActive(true);
            _windowAnimation.Open();
            _windowAnimation.StopTime();
        }

        public void CloseWindow()
        {
            _windowAnimation.Close();
            _windowAnimation.StartTime();
        }
    }
}