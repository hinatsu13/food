using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    public void LoadNextScene(string name)
    {
        Debug.Log("Load next Scene");

        SceneManager.LoadScene(name);

        StateManager.SendPacket();
    }
    public void SetCount(int count)
    {
        StateManager.setStageCount(count);
        StateManager.SendPacket();
    }
}
