using UnityEngine;

/// <summary>
/// Attach to EndScreen GameObject in Fish Selection scene.
/// Updates StateManager with the final Fish Selection score
/// the moment EndScreen becomes visible.
/// </summary>
public class FSel_EndScreen : MonoBehaviour
{
    private void OnEnable()
    {
        // Save the final score to StateManager when EndScreen is shown
        StateManager.setFishSelection(FSel_ScoreManager.selectionScore);
        Debug.Log("[FSel_EndScreen] Fish Selection score saved: " + FSel_ScoreManager.selectionScore);
    }
}
