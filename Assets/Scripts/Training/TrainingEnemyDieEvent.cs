using EnemyLogic;
using Timelines;
using UnityEngine;

public class TrainingEnemyDieEvent : MonoBehaviour
{
    [SerializeField] private EnemyDamageTaker _damageTacker;
    [SerializeField] private TrainingTaskController _taskController;
    [SerializeField] private TimelineController _timelineController;
    [SerializeField] private GameObject _cutscene;

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
    }
}
