using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Makes the thermometer pen/probe draggable.
/// Drop detection is based on the PEN TIP position, not the mouse cursor.
/// During drag, highlights any FishSpotDropTarget under the tip.
/// On drop, triggers the spot under the tip.
/// </summary>
public class DraggablePen : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tip Offset (normalized, from center)")]
    [Tooltip("Offset from center of pen image to the tip, in normalized coordinates.\n" +
             "(-0.5, -0.5) = bottom-left corner, (0, -0.5) = bottom-center")]
    [SerializeField] private Vector2 tipOffsetNormalized = new Vector2(-0.3f, -0.4f);

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Camera canvasCamera;

    // Track currently highlighted spot to unhighlight when leaving
    private FishSpotDropTarget currentHighlightedSpot;

    [HideInInspector] public bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;

        // Cache the canvas camera for raycasting
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvasCamera = canvas.worldCamera;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the pen following the pointer
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Check if the pen tip is hovering over a spot and highlight it
        UpdateTipHighlight();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Clear any highlight
        ClearHighlight();

        // Raycast from the pen TIP position to find drop targets
        FishSpotDropTarget spot = FindSpotAtTip();
        if (spot != null)
        {
            spot.TriggerDrop();
        }

        // Snap back to original position
        rectTransform.anchoredPosition = originalPosition;
    }

    /// <summary>
    /// During drag, highlight any spot under the pen tip.
    /// </summary>
    private void UpdateTipHighlight()
    {
        FishSpotDropTarget spot = FindSpotAtTip();

        if (spot != currentHighlightedSpot)
        {
            // Unhighlight previous
            if (currentHighlightedSpot != null)
                currentHighlightedSpot.SetHighlight(false);

            // Highlight new
            if (spot != null)
                spot.SetHighlight(true);

            currentHighlightedSpot = spot;
        }
    }

    private void ClearHighlight()
    {
        if (currentHighlightedSpot != null)
        {
            currentHighlightedSpot.SetHighlight(false);
            currentHighlightedSpot = null;
        }
    }

    /// <summary>
    /// Gets the screen position of the pen tip based on the normalized offset.
    /// </summary>
    private Vector2 GetTipScreenPosition()
    {
        // Calculate the tip position in local space
        Vector2 size = rectTransform.rect.size;
        Vector2 tipLocal = new Vector2(
            tipOffsetNormalized.x * size.x,
            tipOffsetNormalized.y * size.y
        );

        // Convert local position to world position
        Vector3 tipWorld = rectTransform.TransformPoint(tipLocal);

        // Convert world position to screen position
        return RectTransformUtility.WorldToScreenPoint(canvasCamera, tipWorld);
    }

    /// <summary>
    /// Raycast at the pen tip screen position and return the FishSpotDropTarget found (if any).
    /// </summary>
    private FishSpotDropTarget FindSpotAtTip()
    {
        Vector2 tipScreenPos = GetTipScreenPosition();

        // Create pointer event at the tip position
        PointerEventData tipEventData = new PointerEventData(EventSystem.current)
        {
            position = tipScreenPos
        };

        // Raycast
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(tipEventData, results);

        // Find any FishSpotDropTarget in the results
        foreach (RaycastResult result in results)
        {
            FishSpotDropTarget dropTarget = result.gameObject.GetComponent<FishSpotDropTarget>();
            if (dropTarget != null)
                return dropTarget;
        }

        return null;
    }

    /// <summary>
    /// Call this to update the stored "home" position if it changes.
    /// </summary>
    public void SetHomePosition(Vector2 pos)
    {
        originalPosition = pos;
    }
}
