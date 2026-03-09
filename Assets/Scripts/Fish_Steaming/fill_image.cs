using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class fill_image : MonoBehaviour
{
    public Image cooldown;
    private bool coolingDown = false;
    public float waitTime = 30.0f;
    private bool stop;
    public UnityEvent sparkle;
    private bool calledEvent = false;

    private void Start()
    {
        cooldown.fillAmount = 0;
        coolingDown = false;
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (stop == false)
        {
            if (coolingDown == false)
            {
                //Reduce fill amount over 30 seconds
                cooldown.fillAmount += 1.0f / waitTime * Time.deltaTime;
            }
            if (coolingDown)
            {
                cooldown.fillAmount -= 0.5f / waitTime * Time.deltaTime;
            }
            
            if (cooldown.fillAmount == 0)
            {
                coolingDown = false;
            }
            
            if (cooldown.fillAmount >= 1)
            {
                coolingDown = true;;
            }
            
            if (cooldown.fillAmount <= 0.42 && cooldown.fillAmount >= 0.4 && coolingDown)
            {
                stop = true;
            }
        }
        else
        {
            if (calledEvent == false)
            {
                TriggerEvent();
            }
        }
    }

    void TriggerEvent()
    {
        calledEvent = true;
        sparkle?.Invoke();
    }
}
