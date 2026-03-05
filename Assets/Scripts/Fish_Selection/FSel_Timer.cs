using TMPro;
using UnityEngine;

public class FSel_Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float currentTime = 60f;
    private bool timerIsRunning = true;

    void Update()
    {
        if (timerIsRunning)
        {
            currentTime -= Time.deltaTime; // Increment the time every frame
            DisplayTime(currentTime);
        }
    }
    public void StopTime()
    {
        timerIsRunning = false;
    }
    void DisplayTime(float timeToDisplay)
    {
        // Calculate minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Format the time string to ensure two digits (00:00)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
