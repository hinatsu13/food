using UnityEngine;
using UnityEngine.UI;

public class Fish_Prep_Handler : MonoBehaviour
{
    public GameObject topFillet;    // Assign in Inspector
    public GameObject bottomFillet;
    public GameObject Gutz;
    public GameObject Tray;

    public void SplitFish()
    {
        Debug.Log("The fish is filleted!");
        
        // Disable the whole fish visuals/collider
        GetComponent<Image>().enabled = false;

        // Enable the cut pieces
        topFillet.SetActive(true);
        bottomFillet.SetActive(true);
        Gutz.SetActive(true);
        Tray.SetActive(true);
    }
}
