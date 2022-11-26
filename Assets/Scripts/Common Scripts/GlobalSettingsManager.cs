using UnityEngine;

public class GlobalSettingsManager : MonoBehaviour
{
    public static GlobalSettingsManager Instance { get; private set; }

    public bool CaptureMouse { get; set; }

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
