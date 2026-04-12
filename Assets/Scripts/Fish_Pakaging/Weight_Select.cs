using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Weight_Select : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     [Header("Call manager")]
    [SerializeField] private PakagingManager Manager;
    [SerializeField] private GameObject RecipeUI;
    [Header("Slider Bar")]
    [SerializeField] public Slider Slider;
    [Header("Continue UI")]
    [SerializeField] public GameObject Continue;

    [Header("Ping Pong Speed")]
    public float speed = 2f;
    
    private bool isCasting = false;
    private float timer = 0f;

    void Update() 
    {
        if (isCasting) 
        {
            // Move the timer forward
            timer += Time.deltaTime * speed;

            // PingPong keeps the value between 0 and 1
            // It automatically reverses when it hits the limit
            float barValue = Mathf.PingPong(timer, 1f);
            Slider.value = barValue;

            // Stop the bar when the player clicks or presses Space
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && RecipeUI.activeSelf == false) 
            {
                StopCasting(barValue);
            }
        }
    }
    public void StartCasting()
    {
        isCasting = true;
    }
    void StopCasting(float finalValue) 
    {
        isCasting = false;
        int value;
        // Determine quality based on how close to '1' (the top) the bar is
        if (finalValue < 0.33f) {
            value = 1;
        } else if(finalValue > 0.33f & finalValue < 0.66f){
            value = 2;
        }
        else
        {
            value = 3;
        }
        Debug.Log(value);
        Manager.SelectWeight(value);
        Continue.SetActive(true);
    }
}
