using System;
using UI.Animation;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(WindowAnimation))]
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private GameObject _window;

        private WindowAnimation _windowAnimation;

        private WindowAnimation Animation
        {
            get
            {
                if (_windowAnimation == null)
                    _windowAnimation = GetComponent<WindowAnimation>();

                return _windowAnimation;
            }
        }

        public event Action Opened
        {
            add => Animation.Opened += value;
            remove => Animation.Opened -= value;
        }

        public event Action Closed
        {
            add => Animation.Closed += value;
            remove => Animation.Closed -= value;
        }

        private void Awake()
        {
            _windowAnimation = GetComponent<WindowAnimation>();
        }

        public virtual void OpenWindow()
        {
            gameObject.SetActive(true);
            Animation.Open();
            Animation.StopTime();
        }

        public void CloseWindow()
        {
            Animation.Close();
            Animation.StartTime();
        }

        public void CloseUnscaledTime()
        {
            Animation.Close();
        }

        public virtual void OpenUnscaledTime()
        {
            gameObject.SetActive(true);
            Animation.Open();
        }
    }
}