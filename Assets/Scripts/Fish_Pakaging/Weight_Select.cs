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
    [Header("Reference")]
    [SerializeField] public GameObject Continue;
    [SerializeField] public RectTransform inputArea;

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
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Began)
                    {
                        StopCasting(barValue, Input.GetTouch(0).position);
                    }
                }else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && RecipeUI.activeSelf == false)
                {
                    StopCasting(barValue, Input.mousePosition);
                }
        }
    }
    public void StartCasting()
    {
        isCasting = true;
    }
    void StopCasting(float finalValue, Vector3 inputLocation) 
    {
        bool isInsideArea = RectTransformUtility.RectangleContainsScreenPoint(inputArea, inputLocation, Camera.main);

        if (isInsideArea)
        {
            isCasting = false;
            int value;

            if (finalValue <= 0.33f) value = 1;
            else if (finalValue <= 0.66f) value = 2;
            else value = 3;

            Debug.Log($"Input Registered in Gameplay Area! Value: {value}");
            Manager.SelectWeight(value);
            Continue.SetActive(true);
        }
        else
        {
            Debug.Log("Click ignored: Outside of the InputArea.");
        }
    }
}
