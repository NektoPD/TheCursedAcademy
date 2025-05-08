using Pools;
using UI;
using UnityEngine;
using Zenject;

namespace Tutorial
{
    public class TutorialExpPointPickUpEvent : MonoBehaviour
    {
        [SerializeField] private TutorialTaskController _taskController;
        [SerializeField] private LevelUpWindow _window;

        private ExpPointPool _expPointPool;

        [Inject]
        private void Contruct(ExpPointPool pool)
        {
            _expPointPool = pool;
        }

        private void OnEnable()
        {
            _expPointPool.Returned += OnReturned;
        }

        private void OnDisable()
        {
            _expPointPool.Returned -= OnReturned;
        }

        private void OnReturned()
        {
            _taskController.ShowNextTask();
            _window.OpenWindow();
        }
    }
}