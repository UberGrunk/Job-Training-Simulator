using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneUIManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerTask> tasks;
    [SerializeField]
    private Vector2 tasksListPosition = new Vector2(130, -30);
    [SerializeField]
    private RectTransform tasksListItemPrefab;
    [SerializeField]
    private RectTransform gameOverBackground;
    [SerializeField]
    private RectTransform infoTextPanel;

    private GameObject checkmark;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI timerText;

    private TextMeshProUGUI allTasksDoneText;
    private TextMeshProUGUI securityBreachText;

    private bool showingGameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        if(tasks == null)
            tasks = new List<PlayerTask>();

        checkmark = tasksListItemPrefab.Find("Checkmark").gameObject;
        titleText = tasksListItemPrefab.Find("Title Text").GetComponent<TextMeshProUGUI>();
        descriptionText = tasksListItemPrefab.Find("Description Text").GetComponent<TextMeshProUGUI>();
        timerText = tasksListItemPrefab.Find("Timer Text").GetComponent<TextMeshProUGUI>();

        allTasksDoneText = gameOverBackground.Find("All Tasks Completed Text").GetComponent<TextMeshProUGUI>();
        securityBreachText = gameOverBackground.Find("Security Breach Text").GetComponent<TextMeshProUGUI>();

        showingGameOverScreen = false;

        RenderPlayerTasksList();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalSettingsManager.Instance.GameOver)
        {
            if (!showingGameOverScreen)
            {
                GlobalSettingsManager.Instance.CaptureMouse = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen();
            }
        }
        else
        {
            UpdatePlayerTasks();
            RemovePlayerTasks();
            RenderPlayerTasksList();
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void StartButtonPressed()
    {
        Cursor.visible = false;
        GlobalSettingsManager.Instance.CaptureMouse = true;
        infoTextPanel.gameObject.SetActive(false);
    }

    private void UpdatePlayerTasks()
    {
        bool allTasksDone = true;

        foreach(PlayerTask task in tasks)
        {
            if (task.TaskDone)
                continue;

            if(task.Prerequisites != null)
            {
                bool prerequisitesDone = true;

                foreach(PlayerTask prerequisite in task.Prerequisites)
                {
                    if(!prerequisite.TaskDone)
                        prerequisitesDone = false;
                }

                if (!prerequisitesDone)
                {
                    allTasksDone = false;
                    continue;
                }
            }

            task.UpdateTaskDone();

            if (task.TaskDone)
                task.IsActive = false;
            else
            {
                allTasksDone = false;
                task.IsActive = true;
            }
        }

        if (allTasksDone)
        {
            GlobalSettingsManager.Instance.AllTasksDone = true;
            GlobalSettingsManager.Instance.GameOver = true;
        }
    }

    private void RenderPlayerTasksList()
    {
        float currentYPos = tasksListPosition.y;

        for(int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].IsActive)
            {
                currentYPos -= 25;
                descriptionText.gameObject.SetActive(true);
                tasksListItemPrefab.sizeDelta = new Vector2(tasksListItemPrefab.sizeDelta.x, 450);
            }
            else
            {
                descriptionText.gameObject.SetActive(false);
                tasksListItemPrefab.sizeDelta = new Vector2(tasksListItemPrefab.sizeDelta.x, 230);
            }

            if (tasks[i].TaskDone)
            {
                checkmark.SetActive(true);
                timerText.gameObject.SetActive(false);
            }
            else
            {
                checkmark.SetActive(false);
                if (tasks[i].InitialTaskTimeSeconds > 0)
                    timerText.gameObject.SetActive(true);
                else
                    timerText.gameObject.SetActive(false);
            }

            tasksListItemPrefab.anchoredPosition = new Vector2(tasksListItemPrefab.anchoredPosition.x, currentYPos);

            titleText.text = tasks[i].TaskName;
            descriptionText.text = tasks[i].TaskDescription;

            int timerMinutes = (int)(tasks[i].TaskTimeSeconds / 60);
            int timerSeconds = (int)(tasks[i].TaskTimeSeconds % 60);
            timerText.text = timerMinutes.ToString() + ":" + (timerSeconds < 10 ? "0" : "") + timerSeconds.ToString();

            Instantiate(tasksListItemPrefab.gameObject, transform, false);

            if (tasks[i].IsActive)
                currentYPos -= 75;
            else
                currentYPos -= 55;
        }
    }

    private void RemovePlayerTasks()
    {
        GameObject[] playerTasks = GameObject.FindGameObjectsWithTag("Player Task");

        foreach(GameObject playerTask in playerTasks)
        {
            Destroy(playerTask);
        }
    }

    private void ShowGameOverScreen()
    {
        if(GlobalSettingsManager.Instance.AllTasksDone)
        {
            allTasksDoneText.gameObject.SetActive(true);
            securityBreachText.gameObject.SetActive(false);
            gameOverBackground.gameObject.SetActive(true);
        }
        else
        {
            allTasksDoneText.gameObject.SetActive(false);
            securityBreachText.gameObject.SetActive(true);
            gameOverBackground.gameObject.SetActive(true);
        }

        showingGameOverScreen = true;
    }
}
