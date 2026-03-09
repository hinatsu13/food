using System;
using UnityEngine;

public class LineChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Dragable_Object check_knife = other.GetComponent<Dragable_Object>();
        if (check_knife != null)
        {
            check_knife.SetValid(false);
        }
    }
}
