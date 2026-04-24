using UnityEngine;
using System.Collections;

public class check_onclick : MonoBehaviour
{
    [SerializeField] int click;
    [SerializeField] GameObject targetPanel;
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

    // Called by the Button's onClick instead of OpenPanel directly.
    // Waits one frame so Popup_panel.Start() finishes before re-activating.
    public void OpenPanelDelayed()
    {
        if (targetPanel != null)
        {
            StartCoroutine(OpenAfterFrame());
        }
    }

    IEnumerator OpenAfterFrame()
    {
        targetPanel.SetActive(true);
        yield return null;
        targetPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
