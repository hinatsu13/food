using UnityEngine;

public class Loading : MonoBehaviour
{
    private static Loading _instance;

    [SerializeField] private GameObject loadingPanel;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        Hide();
    }

    public static void Show()
    {
        if (_instance != null && _instance.loadingPanel != null)
            _instance.loadingPanel.SetActive(true);
    }

    public static void Hide()
    {
        if (_instance != null && _instance.loadingPanel != null)
            _instance.loadingPanel.SetActive(false);
    }
}
