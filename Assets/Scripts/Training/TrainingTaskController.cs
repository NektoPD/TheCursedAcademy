using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrainingTaskController : MonoBehaviour
{
    [SerializeField] private List<Image> _tasks;
    [SerializeField] private Image _currentTask;

    private List<Image> _currentTasks;
    private Image _task;

    private void Awake()
    {
        _currentTasks = _tasks;
        _task = _currentTask;
    }

    public void ShowNextTask()
    {
        if (_currentTasks.Count == 0)
            return;

        _task.gameObject.SetActive(false);

        _task = _currentTasks.First();
        _currentTasks.Remove(_task);

        _task.gameObject.SetActive(true);
    }
}
