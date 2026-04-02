using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drop target for fish spots. When the pen is dropped here,
/// it triggers the temperature check via the game manager.
/// </summary>
public class FishSpotDropTarget : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int spotIndex; // 0=head, 1=middle, 2=tail
    [HideInInspector] public FishCheckTempManager manager;

    private RectTransform rectTransform;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Check if the dropped object is the pen
        DraggablePen pen = eventData.pointerDrag?.GetComponent<DraggablePen>();
        if (pen != null && manager != null)
        {
            manager.OnPenDroppedOnSpot(spotIndex);
        }
        // Reset highlight
        rectTransform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only highlight when something is being dragged
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<DraggablePen>() != null)
        {
            rectTransform.localScale = originalScale * 1.3f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = originalScale;
    }
}
