using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUIManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerTask> tasks;

    // Start is called before the first frame update
    void Start()
    {
        if(tasks == null)
            tasks = new List<PlayerTask>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
