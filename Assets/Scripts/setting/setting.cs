using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour
{
    private bool isPaused = false;
    void Awake()
    {
        Time.timeScale = 1f;
    }
    void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
