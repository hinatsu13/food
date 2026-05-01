using UnityEngine;
using UnityEngine.UI;

public class StageLock : MonoBehaviour
{
    [SerializeField] private int stageRec;
    private Image buttonColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonColor = GetComponent<Image>();
        if (!StateManager.isPlayable(stageRec))
        {
            disabledSelection();
        }
    }

    public void disabledSelection()
    {
        buttonColor.color = Color.HSVToRGB(0f, 0f, 0.4f);
        GetComponent<Button>().enabled = false;
    }
}
