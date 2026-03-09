using UnityEngine;
using UnityEngine.UI;

public class Fish_Prep_Handler : MonoBehaviour
{
    [SerializeField]
    private GameObject Panel;
    [SerializeField]
    private GameObject topFillet;    // Assign in Inspector
    [SerializeField]
    private GameObject bottomFillet;
    [SerializeField]
    private GameObject Gutz;
    [SerializeField]
    private GameObject GutzBox;
    [SerializeField]
    private GameObject Tray;
    [SerializeField]
    private GameObject GuideLine;
    [SerializeField]
    private GameObject DragAbleObjective;
    [SerializeField]
    private GameObject showerHead;
    [SerializeField]
    private GameObject cleanFishCheck;
    [SerializeField]
    private GameObject showerBoxA;
    [SerializeField]
    private GameObject showerBoxB;
    [SerializeField]
    private GameObject sparkle;

    public void SplitFish()
    {
        Debug.Log("The fish is filleted!");
        // Disable the fish
        DragAbleObjective.SetActive(false);
        // OnlyImage Otherwise the event not working (Maybe)
        GetComponent<Image>().enabled = false;
        GuideLine.SetActive(false);
        
        // Enable the cut pieces
        topFillet.SetActive(true);
        bottomFillet.SetActive(true);
        Gutz.SetActive(true);
        Tray.SetActive(true);
        GutzBox.SetActive(true);
        
    }

    public void CleanFish()
    {
        //Turn Off Old guts
        Gutz.SetActive(false);
        Tray.SetActive(false);
        GutzBox.SetActive(false);
        
        //Active new minigame
        showerHead.SetActive(true);
        cleanFishCheck.SetActive(true);
        showerBoxA.SetActive(true);
        showerBoxB.SetActive(true);
    }

    public void Cutscene_Trigger()
    {
        showerHead.SetActive(false);
        cleanFishCheck.SetActive(false);
        showerBoxA.SetActive(false);
        showerBoxB.SetActive(false);
        sparkle.SetActive(true);
    }
}
