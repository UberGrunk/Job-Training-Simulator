using UnityEngine;

public class GlobalSettingsManager : MonoBehaviour
{
    public static GlobalSettingsManager Instance { get; private set; }

    public bool CaptureMouse { get; set; }
    public bool GameOver { get; set; }
    public bool AllTasksDone { get; set; }
    public GameObject LookedAtObject { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
