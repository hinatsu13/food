using UnityEngine;
using TMPro;

public class ShowName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        if (text == null) return;

        string playerName = StateManager.getPlayerName();
        text.text = string.IsNullOrEmpty(playerName) ? "---" : playerName;
    }
}
