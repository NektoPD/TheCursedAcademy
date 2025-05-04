using Data;
using Pools;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

namespace Training
{
    public class TrainingExpPointPickUpEvent : MonoBehaviour
    {
        [SerializeField] private TrainingTaskController _taskController;
        [SerializeField] private LevelUpWindow _window;
        [SerializeField] private List<ItemVisualData> _items;

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
            _window.ShowWithItems(_items);
        }
    }
}