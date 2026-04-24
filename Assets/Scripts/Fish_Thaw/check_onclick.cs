using UnityEngine;

public class check_onclick : MonoBehaviour
{
    [SerializeField] int click;
    int click_count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnButtonClick()
    {
        click_count++; // Increment the counter every time the button is clicked

        if (click_count == click)
        {
            ShowObject();
        }
    }

    void ShowObject()
    {
        gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
