using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable_Object : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject Dropbox;
    public Image image;
    public Transform parentAfterDrag;
    private RectTransform _rectTransform;
    private bool _onCutValid = false;
    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(p: transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        SetValid(true);
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    public void SetValid(bool valid)
    {
        _onCutValid = valid;
    }
    
    public bool ValidateCut()
    {
        Debug.Log("ValidateCut");
        return _onCutValid;
    }
}
