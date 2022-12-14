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
    private float taskTimeSeconds;
    [SerializeField]
    private int initialTaskTimeSeconds;

    public bool IsActive { get { return isActive; } set { isActive = value; } }
    public bool TaskDone { get { return taskDone; } set { taskDone = value; } }
    public string TaskName { get { return taskName; } }
    public string TaskDescription { get { return taskDescription; } }
    public List<PlayerTask> Prerequisites { get { return prerequisites; } }
    public float TaskTimeSeconds { get { return taskTimeSeconds; } set { taskTimeSeconds = value; } }
    public int InitialTaskTimeSeconds { get { return initialTaskTimeSeconds; } }

    public void UpdateTaskDone()
    {
        conditionsTest.Invoke(this);
    }
}
