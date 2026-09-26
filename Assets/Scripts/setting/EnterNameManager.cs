using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Logs the player in by Employee ID. There is no roster: any well-formed ID
// is accepted (the organisation enforces correct IDs), and the ID need not
// pre-exist — the server creates the player's record on the first score
// save. The input field and class name are kept as "name" for scene-reference
// safety, but the value entered is the Employee ID.
public class EnterNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    public void ValidName()
    {
        string raw = nameInput != null ? nameInput.text : "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            if (errorText != null) errorText.text = "Please enter your Employee ID";
            return;
        }

        string personCode = MongoDBService.NormalizeEmployeeId(raw);
        if (personCode == null)
        {
            if (errorText != null) errorText.text = "Invalid Employee ID";
            return;
        }

        Loading.Show();

        MongoDBService.GetPerson(personCode, (response) =>
        {
            if (response == null)
            {
                // Client-side validation already passed, so this is a network
                // or server failure (or a server-side 400 on a rule mismatch).
                Loading.Hide();
                if (errorText != null) errorText.text = "Cannot connect to server. Please try again.";
                Debug.Log("[EnterName] Login failed for Employee ID: " + personCode);
                return;
            }

            StateManager.setPlayerCode(response.person.personCode);

            // Rehydrate game state from server (zeros for first-time players).
            if (response.gameData != null)
            {
                StateManager.setFishSelection(response.gameData.fishSelectionScore);
                StateManager.setFishPrep(response.gameData.fishPrepScore);
                StateManager.setFishCheckTemp(response.gameData.fishCheckTempScore);
                StateManager.setFishPackaging(response.gameData.fishPackagingScore);
                StateManager.setStageCount(response.gameData.stageCount);
            }

            Debug.Log("[EnterName] Welcome " + response.person.personCode);

            Loading.Hide();
            SceneManager.LoadScene("Scene_Selector");
        });
    }
}
