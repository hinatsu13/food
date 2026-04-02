using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drop target for fish spots. The pen tip detection is handled
/// entirely by DraggablePen - this script just receives the call.
/// No IDropHandler so Unity's built-in mouse-based drop doesn't interfere.
/// </summary>
public class FishSpotDropTarget : MonoBehaviour
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

    /// <summary>
    /// Called by DraggablePen when the pen tip overlaps this spot on drop.
    /// </summary>
    public void TriggerDrop()
    {
        if (manager != null)
        {
            manager.OnPenDroppedOnSpot(spotIndex);
        }
        ResetHighlight();
    }

    public void SetHighlight(bool highlighted)
    {
        if (rectTransform != null)
            rectTransform.localScale = highlighted ? originalScale * 1.3f : originalScale;
    }

    public void ResetHighlight()
    {
        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }
}
