using UnityEngine;
using UnityEngine.UI;

public class infomation : MonoBehaviour
{
    public RectTransform targetButtonRect;

    public void OpenImage()
    {
        Debug.Log($"[info] Open  {name} pos->570");
        gameObject.SetActive(true);
        ChangeButtonLocation();
    }

    public void CloseImage()
    {
        Debug.Log($"[info] Close {name} pos->970");
        ResetLocation();
        gameObject.SetActive(false);
    }

    public void ChangeButtonLocation()
    {
        targetButtonRect.anchoredPosition = new Vector2(570, 645);
    }

    public void ResetLocation()
    {
        targetButtonRect.anchoredPosition = new Vector2(970, 645);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
