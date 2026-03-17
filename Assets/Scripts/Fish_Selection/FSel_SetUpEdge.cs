using UnityEngine;

public class FSel_SetUpEdge : MonoBehaviour
{
    public GameObject indicator;
    public float displayTime;
    void Awake()
    {
        FSel_ScoreManager.edge = this;
    }
    public void displayIndicator()
    {
        GameObject _indicator = Instantiate(indicator, transform);
        Destroy(_indicator, displayTime);
    }
}
