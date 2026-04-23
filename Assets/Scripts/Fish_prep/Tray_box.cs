using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tray_Box : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private UnityEvent calledEvent;
    [SerializeField]
    private Fish_Prep_Handler Handler;
    [Tooltip("For storing the placement of each guts objects.")]
    [SerializeField] private RectTransform[] gutPlacement;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Dragable_Object Guts = dropped.GetComponent<Dragable_Object>();
        Debug.Log("Getting a Guts");
        if (Guts.ValidateCut())
        {
            Debug.Log("Calling Event");
            dropped.transform.position = gutPlacement[Handler.Guts_Count].position;
            dropped.transform.SetParent(gutPlacement[Handler.Guts_Count]);
            Guts.enabled = false;
            //Guts.parentAfterDrag = transform;
            //Destroy(dropped);
            Handler.Guts_Count += 1;
        }
        if(Handler.Guts_Count == 5)
        {
            calledEvent?.Invoke();
        }
        
    }
}
