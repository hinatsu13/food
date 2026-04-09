using UnityEngine;

public class close_in_time : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float delay = 10.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(HideObject), delay);
    }

    void HideObject()
    {
        targetObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
