using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tray_Box : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Gutz guts = dropped.GetComponent<Gutz>();
        guts.parentAfterDrag = transform;
        Debug.Log("Fishnish the stage");
    }
    void Update()
    {

    }
}
