using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fill_image : MonoBehaviour
{
    public Image cooldown;
    private bool coolingDown = false;
    public float waitTime = 30.0f;
    private bool stop;

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
            
            if (cooldown.fillAmount <= 0.42 && cooldown.fillAmount >= 0.4)
            {
                stop = true;
            }
        }
    }
}
