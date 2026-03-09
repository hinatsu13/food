using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class End_Box : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private UnityEvent calledEvent;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Dragable_Object knife = dropped.GetComponent<Dragable_Object>();
        Debug.Log("Getting a knife");
        if (knife.ValidateCut())
        {
            Debug.Log("Calling Event");
            knife.parentAfterDrag = transform;
            calledEvent.Invoke();
        }
        
    }
}
