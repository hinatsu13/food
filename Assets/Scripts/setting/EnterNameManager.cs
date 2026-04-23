using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnterNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    public void ValidName()
    {
        if (nameInput.text.Length > 0)
        {
            string playerName = nameInput.text;
            StateManager.setPlayerName(playerName);

            Loading.Show();

            // Check if player exists in DB → load their scores or create new
            MongoDBService.GetPlayer(playerName, (playerData) =>
            {
                if (playerData != null)
                {
                    // Returning player — restore their scores
                    Debug.Log("[EnterName] Welcome back, " + playerName + "!");
                    StateManager.setFishSelection(playerData.fishSelectionScore);
                    StateManager.setFishPrep(playerData.fishPrepScore);
                    StateManager.setFishCheckTemp(playerData.fishCheckTempScore);
                    StateManager.setFishPackaging(playerData.fishPackagingScore);
                }
                else
                {
                    // New player — create record with zeros
                    Debug.Log("[EnterName] New player: " + playerName);
                    MongoDBService.SendScore(playerName, 0, 0, 0, 0);
                }

                Loading.Hide();
                SceneManager.LoadScene("Scene_Selector");
            });
        }
        else
        {
            errorText.text = "Please enter your name";
        }
    }
}
