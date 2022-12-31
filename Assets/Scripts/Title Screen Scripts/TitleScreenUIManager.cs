using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUIManager : MonoBehaviour
{
    private void Start()
    {
        GlobalSettingsManager.Instance.CaptureMouse = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SteamEngineButtonClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void BakeryButtonClicked()
    {

    }

    public void NuclearPowerButtonClicked()
    {

    }
}
