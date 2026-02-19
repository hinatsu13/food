using System;
using UnityEngine;

public class FSel_InputDetector : MonoBehaviour
{
    [Header("Objective Condition")]
    [SerializeField] public int Species;//to check if the chosen fish is correct, Match it with species value in FSel_Fish
    [Header("Config")]
    [SerializeField] private float Result_Display_Lifetime = 0.5f;
    [SerializeField] private GameObject correctIndicator;
    [SerializeField] private GameObject incorrectIndicator;

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void ShowResult(bool isCorrect)
    {
        if (isCorrect)
        {
            GameObject _result = Instantiate(correctIndicator, transform);
            Destroy(_result, Result_Display_Lifetime);
        }
        else
        {
            GameObject _result = Instantiate(incorrectIndicator, transform);
            Destroy(_result, Result_Display_Lifetime);
        }
            
    }
}
