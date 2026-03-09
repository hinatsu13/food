using UnityEngine;

public class show_buttondelay : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private float delay = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(ShowButton), delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerButton()
    {
        Invoke(nameof(ShowButton), delay);
    }
    void ShowButton()
    {
        button.SetActive(true);
    }
}
