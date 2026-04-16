using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class EnterNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ValidName()
    {
        if (nameInput.text.Length > 0)
        {
            StateManager.setPlayerName(nameInput.text);
            SceneManager.LoadScene("Scene_Selector");
        } else {
            errorText.text = "Please enter your name";
        }
    }
}
