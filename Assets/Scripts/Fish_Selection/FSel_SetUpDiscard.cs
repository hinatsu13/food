using UnityEngine;

public class FSel_SetUpDiscard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public FSel_InputDetector indicator;
    void Awake()
    {
        FSel_ScoreManager.discardIndicator = indicator;
    }
}
