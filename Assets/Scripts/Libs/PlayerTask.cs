using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTask : MonoBehaviour
{
    [SerializeField]
    private bool taskDone = false;
    [SerializeField]
    private string taskName;
    [SerializeField]
    private string taskDescription;
    [SerializeField]
    private bool isActive;
    [SerializeField]
    private UnityEvent<PlayerTask> conditionsTest;
    [SerializeField]
    private List<PlayerTask> prerequisites;
    [SerializeField]
    private int taskTimeSeconds;
    [SerializeField]
    private int initialTaskTimeSeconds;

    public void UpdateTaskDone()
    {
        conditionsTest.Invoke(this);
    }

    public void SetTaskDone(bool newTaskDoneValue)
    {
        taskDone = newTaskDoneValue;
    }
}
