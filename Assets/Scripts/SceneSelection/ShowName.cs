using UnityEngine;
using TMPro;

// Shows the logged-in Employee ID (there are no player names).
public class ShowName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        if (text == null) return;

        string playerCode = StateManager.getPlayerCode();
        text.text = string.IsNullOrEmpty(playerCode) ? "---" : playerCode;
    }
}
