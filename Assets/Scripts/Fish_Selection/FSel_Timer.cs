using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FSel_Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float maxTime = 60f;
    private float currentTime;
    private bool timerIsRunning = false;

    public UnityEvent OnEnd;
    //[SerializeField] protected TextMeshProUGUI EndScore;

    private void Awake()
    {
        Time.timeScale = 0;
        currentTime = maxTime;
        FSel_ScoreManager.timer = this;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            currentTime -= Time.deltaTime; // Increment the time every frame
            DisplayTime(currentTime);
        }
        if(currentTime <= 0)
        {
            //EndScore.text = FSel_ScoreManager.selectionScore.ToString();
            timerText.text = $"--:--";
            StopTime();
            OnEnd?.Invoke();
        }
    }
    public void StopTime()
    {
        timerIsRunning = false;
        Time.timeScale = 0;
    }

    public void StartTime()
    {
        timerIsRunning = true;
        Time.timeScale = 1;
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
