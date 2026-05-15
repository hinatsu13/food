using Unity.VisualScripting;
using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    [Header("---------Music Changer----------")]
    public AudioSource Music_Theme;
    public static Music_Manager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() {
        if (instance != null)
        {
            // Compare the actual song files (clips)
            if (instance.Music_Theme.clip == Music_Theme.clip)
            {
                // SAME SONG: Kill this new one so the music doesn't restart
                Destroy(gameObject);
                return;
            }
            else
            {
                // DIFFERENT SONG: Kill the old one to let the new theme play
                Destroy(instance.gameObject);
            }
        }
        // Set up the new instance
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Make sure the music actually plays if it's the new theme
        if (!Music_Theme.isPlaying)
        {
            Music_Theme.Play();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
