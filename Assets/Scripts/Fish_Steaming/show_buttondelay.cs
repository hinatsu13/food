using UnityEngine;

public class show_buttondelay : MonoBehaviour
{
    [SerializeField] private GameObject button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(ShowButton), 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowButton()
    {
        button.SetActive(true);
    }
}
