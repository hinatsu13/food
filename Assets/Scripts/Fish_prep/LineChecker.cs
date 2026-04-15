using System;
using UnityEngine;

public class LineChecker : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        Dragable_Object check_knife = other.GetComponent<Dragable_Object>();
        if (check_knife != null)
        {
            check_knife.SetValid(false);
        }
    }
}
