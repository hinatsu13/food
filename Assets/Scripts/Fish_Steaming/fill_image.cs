using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fill_image : MonoBehaviour
{
    public Image cooldown;
    public bool coolingDown;
    public float waitTime = 30.0f;

    // Update is called once per frame
    void Update()
    {
        if (coolingDown == true)
        {
            //Reduce fill amount over 30 seconds
            cooldown.fillAmount -= 1.0f / waitTime * Time.deltaTime;
        }
    }
}
