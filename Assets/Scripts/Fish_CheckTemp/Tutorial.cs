using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
    }
    public void startGame()
    {
        Time.timeScale = 1.0f;
    }
}
