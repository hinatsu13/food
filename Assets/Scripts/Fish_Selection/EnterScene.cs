using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void LoadNextScene(string name)
    {
        Debug.Log("Load next Scene");
        //SceneManager.LoadScene(name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
