using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Drop_Box : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        knife knife = dropped.GetComponent<knife>();
        knife.parentAfterDrag = transform;
    }
}
