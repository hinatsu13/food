using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Authenticates the player by PersonCode against the HRIS roster. The input
// field and class name are kept as "name" for scene-reference safety, but
// the value entered is treated as a PersonID / PersonCode.
public class EnterNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text errorText;

    public void ValidName()
    {
        string personCode = nameInput != null ? nameInput.text.Trim() : "";

        if (string.IsNullOrEmpty(personCode))
        {
            if (errorText != null) errorText.text = "Please enter your Employee ID";
            return;
        }

        Loading.Show();

        MSSqlService.GetPerson(personCode, (response) =>
        {
            if (response == null)
            {
                // 404 from server, or a network failure.
                Loading.Hide();
                if (errorText != null) errorText.text = "Employee ID not found";
                Debug.Log("[EnterName] Login rejected for PersonCode: " + personCode);
                return;
            }

            // Identity: store the canonical PersonCode plus a display name.
            StateManager.setPlayerCode(response.person.personCode);
            StateManager.setPlayerName(BuildDisplayName(response.person));

            // Rehydrate game state from server (zeros for first-time players).
            if (response.gameData != null)
            {
                StateManager.setFishSelection(response.gameData.fishSelectionScore);
                StateManager.setFishPrep(response.gameData.fishPrepScore);
                StateManager.setFishCheckTemp(response.gameData.fishCheckTempScore);
                StateManager.setFishPackaging(response.gameData.fishPackagingScore);
                StateManager.setStageCount(response.gameData.stageCount);
            }

            Debug.Log("[EnterName] Welcome " + StateManager.getPlayerName()
                + " (PersonCode " + response.person.personCode + ")");

            Loading.Hide();
            SceneManager.LoadScene("Scene_Selector");
        });
    }

    // Display name preference: NickName → "FnameE LnameE" → PersonCode.
    private static string BuildDisplayName(MSSqlService.PersonInfo person)
    {
        if (!string.IsNullOrEmpty(person.nickName))
            return person.nickName;

        string first = person.fnameE ?? "";
        string last  = person.lnameE ?? "";
        string combined = (first + " " + last).Trim();
        if (!string.IsNullOrEmpty(combined))
            return combined;

        return person.personCode;
    }
}
