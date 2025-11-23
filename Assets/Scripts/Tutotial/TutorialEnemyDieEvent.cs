using System;
using EnemyLogic;
using Timelines;
using UnityEngine;

namespace Tutorial
{
    public class TutorialEnemyDieEvent : MonoBehaviour
    {
        [SerializeField] private EnemyDamageTaker _damageTacker;
        [SerializeField] private TutorialTaskController _taskController;
        [SerializeField] private TimelineController _timelineController;
        [SerializeField] private GameObject _cutscene;

        public event Action TutorialEnemyDied;
        
        private void OnEnable()
        {
            _damageTacker.Health.Died += Die;
        }

        private void OnDisable()
        {
            _damageTacker.Health.Died -= Die;
        }

        private void Die()
        {
            _timelineController.StartCutscene(_cutscene.name);
            _taskController.ShowNextTask();
            TutorialEnemyDied?.Invoke();
        }
    }
}