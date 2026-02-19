using UnityEngine;
using UnityEngine.UI;

public class infomation : MonoBehaviour
{
    public RectTransform targetButtonRect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //info.text = "what the fox say";
        gameObject.SetActive(false);
    }

    public void OpenImage()
    {
        gameObject.SetActive(true);
    }

    public void CloseImage()
    {
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
