using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class knife : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject Dropbox;
    public Image image;
    public Transform parentAfterDrag;
    public LayerMask fishLayer;
    private Vector2 _startDragPos;
    private RectTransform _rectTransform;
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _startDragPos = eventData.position;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        ValidateCut(_startDragPos, eventData.position);
    }
    private void ValidateCut(Vector2 start, Vector2 end)
    {
        // 1. Calculate the midpoint or path of the drag
        // For UI, we can use GraphicRaycaster or a simple Physics2D raycast if using a ScreenSpace camera
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Vector2.Lerp(start, end, 0.5f) // Check the middle of the swipe
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Fish")) 
            {
                Debug.Log(Vector2.Distance(start, end));
                // Check if the drag distance was long enough to be a "swipe"
                if (Vector2.Distance(start, end) > 15f) 
                {
                    Debug.Log("Valid Cut on: " + result.gameObject.name);
                    result.gameObject.GetComponent<Fish_Prep_Handler>()?.SplitFish();
                    gameObject.SetActive(false);
                    Dropbox.SetActive(false);
                    break;
                }
            }
        }
    }
}
